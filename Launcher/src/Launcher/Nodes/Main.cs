#region

using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Common;
using Common.Api;
using Common.FileSystem.Deploy;
using Common.IO.Checksum;
using HashedFiles;
using IO;
using KludgeBox.Events.Global;
using KludgeBox.SaveLoad.Nbt;
using KludgeBox.Scheduling;
using Launcher.Tasks;
using Launcher.Tasks.Environment;
using Launcher.Tasks.Environment.Core;
using Launcher.Tasks.Environment.Mods;
using Launcher.Tasks.Info;
using Launcher.Tasks.Launch;
using PatchApi;
using PatchApi.Events;
using WebApi;

#endregion

namespace Launcher.Nodes;

public partial class Main : Node
{
	public static IApiProvider ApiProvider { get; set; }
	public static IChecksumProvider ChecksumProvider { get; set; }
	public static IFileDeployer FileDeployer { get; set; }
	public static Main Instance { get; private set; }
	public static VersionInfo AppVersion { get; private set; }
	public static TaskManager TaskManager { get; private set; }
	public static Scheduler Scheduler { get; private set; }
	public static Settings Settings { get; private set; }
	public static LauncherState State { get; private set; }
	public static PatchManager PatchManager { get; private set; }
	public static TagCompound CustomData { get; private set; }
	public static Popup Popup => Instance._popup;
	public static Notifications Notifications => Instance._notifications;
	public static OptionalModsMenu OptionalModsMenu => Instance._optionalModsMenu;

	[Export] public LineEdit PlayerNameTextBox;
	[Export] public LineEdit PasswordTextBox;
	[Export] public Label RunningTasksLabel;
	[Export] public Label PendingTasksLabel;
	[Export] public Label RamLabel;
	[Export] public Label VersionInfoLabel;
	[Export] public Button RunButton;
	[Export] public Button ModsButton;
	[Export] public HSlider RamSlider;
	[Export] public VBoxContainer RunningTasksContainer;
	[Export] public VBoxContainer PendingTasksContainer;
	[Export] public PanelContainer ConfigPanel;
	[Export] public PackedScene TaskTrackerScene;
	
	// Interactive part
	[Export] private Popup _popup;
	[Export] private OptionalModsMenu _optionalModsMenu;
	[Export] private Notifications _notifications;
	
	public override async void _Ready()
	{
		Instance = this;
		AppVersion = new VersionInfo(new Version(1, 2, 0));
		Settings = SettingsUtils.LoadSettings();
		PatchManager = new PatchManager();
		InitNbtSystems();
		LoadCustomData();
		
		Scheduler = new Scheduler();
		TaskManager = new TaskManager(Scheduler);
		ApiProvider = new DefaultApiProvider();
		State = new LauncherState();
		
		ChecksumProvider = DefaultServices.ChecksumProvider;
		FileDeployer = DefaultServices.FileDeployer;
		
		SettingsUtils.SaveSettings(Settings);
		Scheduler.PeriodicInSeconds(0.1, UpdateButton);
		SetupUi();

		if(!CmdArgsService.ContainsInCmdArgs(CmdArgs.SkipPatchKey))
			await LoadPatches();
		
		EventBus.Publish(new CreatingMainTasksEvent());

		// Prepare version info
		var coreVersionTag = String.IsNullOrWhiteSpace(AppVersion.CoreVersionTag) ? "" : $"-{AppVersion.CoreVersionTag}";
		var patchVersionTag = String.IsNullOrWhiteSpace(AppVersion.PatchVersionTag) ? "" : $"-{AppVersion.CoreVersionTag}";
		var patchVersionFull = AppVersion.PatchVersion is null ? "" : $"патч {AppVersion.PatchVersion}-{AppVersion.PatchVersion}";
		VersionInfoLabel.Text = $"Версия {AppVersion.CoreVersion}{coreVersionTag} {patchVersionFull}";
		
		// Apply window title
		DisplayServer.WindowSetTitle($"Паштетный лаунчер — {VersionInfoLabel.Text}: {TitleMessages.MessagePicker.Pick()}");
		
		// Prepare initialization tasks
		var prepareTask = new PrepareFilesystemTask();
		var serverCheckTask = new PingServerTask();
		var cleanupDownloadsTask = new CleanupBrokenDownloads().AfterTasks(prepareTask);
		var jreTask = new PrepareJreTask().AfterTasks(prepareTask, cleanupDownloadsTask);
		var minecraftTask = new PrepareMinecraftTask().AfterTasks(prepareTask, cleanupDownloadsTask);
		var finishTask = new FinishPreparationsTask().AfterTasks(jreTask, minecraftTask);

		LauncherTask[] preparedTasks = [serverCheckTask, prepareTask, cleanupDownloadsTask, jreTask, minecraftTask, finishTask];
		
		// Check and run
		EventBus.Publish(new RunningMainTasksOnTaskManagerEvent(preparedTasks));
		TaskManager.AddTasks(preparedTasks);
	}

	private void LoadCustomData()
	{
		TagCompound customDataTag;
		try
		{
			var customDataBuffer = File.ReadAllBytes(Paths.CustomDataFilePath.AsAbsolute());
			customDataTag = TagIO.FromBuffer(customDataBuffer);
		}
		catch (Exception ex)
		{
			Log.Warning($"Failed to load custom data: {ex.Message}");
			customDataTag = new TagCompound();
		}
		
		CustomData = customDataTag;
		SaveCustomData();
	}

	private void SaveCustomData()
	{
		var customDataBuffer = TagIO.ToBuffer(CustomData);
		File.WriteAllBytes(Paths.CustomDataFilePath.AsAbsolute(), customDataBuffer);
	}

	private void UpdateButton()
	{
		if(!EventBus.PublishIsCancelled(new RunButtonAboutToUpdate(RunButton)))
			RunButton.Disabled = !State.CanLaunch;
	}

	public override void _Process(double delta)
	{
		Scheduler.Update(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		var shownRunningTasks = RunningTasksContainer.GetChildren().OfType<TaskTracker>().ToList();
		var shownPendingTasks = PendingTasksContainer.GetChildren().OfType<TaskTracker>().ToList();

		var missingRunningTasks = TaskManager.RunningTasks.Where(t => t.IsVisible).Except(shownRunningTasks.Select(tt => tt.TrackingTask));
		var missingPendingTasks = TaskManager.PendingTasks.Where(t => t.IsVisible).Except(shownPendingTasks.Select(tt => tt.TrackingTask));

		var runningTasksToHide = shownRunningTasks.Where(tt => !TaskManager.RunningTasks.Where(t => t.IsVisible).Contains(tt.TrackingTask));
		var pendingTasksToHide = shownPendingTasks.Where(tt => !TaskManager.PendingTasks.Where(t => t.IsVisible).Contains(tt.TrackingTask));

		foreach (var pendingTask in missingPendingTasks)
		{
			var pendingTracker = TaskTrackerScene.Instantiate() as TaskTracker;
			pendingTracker!.From(pendingTask);
			PendingTasksContainer.AddChild(pendingTracker);
		}
		
		foreach (var runningTask in missingRunningTasks)
		{
			var runningTracker = TaskTrackerScene.Instantiate() as TaskTracker;
			runningTracker!.From(runningTask);
			RunningTasksContainer.AddChild(runningTracker);
		}

		var tasksToRemove = runningTasksToHide.Concat(pendingTasksToHide);
		foreach (var taskTracker in tasksToRemove)
		{
			taskTracker.QueueFree();
		}

		PendingTasksLabel.Text = $"Задач в очереди: {TaskManager.PendingTasks.Count} ({TaskManager.PendingTasks.Count(t => !t.IsVisible)} скрыто)";
		RunningTasksLabel.Text = $"Задач запущено: {TaskManager.RunningTasks.Count} ({TaskManager.RunningTasks.Count(t => !t.IsVisible)} скрыто)";
	}

	public static double GetInstalledRamAmount()
	{
		var info = GC.GetGCMemoryInfo();
		var gigs = Mathf.RoundToInt(info.TotalAvailableMemoryBytes / 1024d / 1024d / 1024d);

		return gigs;
	}
	
	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest)
		{
			SaveCustomData();
			SettingsUtils.SaveSettings(Settings);
			GetTree().Quit(); // default behavior
		}
	}

	private async Task LoadPatches()
	{
		Log.Info("Установка патчей...");

		LocalFile localPatchInfo;
		if (CmdArgsService.ContainsInCmdArgs(CmdArgs.DebugPatchKey))
		{
			var paths = new[]
			{
				"../LauncherPatches/bin/Debug/net8.0/LauncherPatches.dll",
				"../LauncherPatches/bin/Release/net8.0/LauncherPatches.dll",
				Paths.PatchFilePath.AsAbsolute()
			};

			var path = paths.FirstOrDefault(File.Exists);
			Log.Warning($"Загружается отладочный патч по пути: {path}");
			localPatchInfo = new LocalFile(path);
		}
		else
		{
			var remotePatchInfo = await ApiProvider.GetPatchInfoAsync();
			localPatchInfo = new LocalFile(Paths.PatchFilePath.AsAbsolute(), ChecksumProvider);
			if (remotePatchInfo is not null && remotePatchInfo.Checksum != localPatchInfo.GetPrecalculatedChecksum())
			{
				bool downloadAccepted = false;
				await Popup.BeginBuild()
					.WithTitle("Доступен новый патч")
					.WithDescription("Новый патч для лаунчера доступен для скачивания.")
					.WithButton("Скачать", () =>
					{
						downloadAccepted = true;
					})
					.WithButton("Пропустить") // just close popup
					.PauseScheduler()
					.EnqueueAndWaitAsync();

				if (downloadAccepted)
				{
					Log.Info("Скачивается патч...");
					var download = new DownloadTask(remotePatchInfo.Url, localPatchInfo.FilePath);
					await download.RunAsync();
				}
			}
		}
		
		
		Assembly currentAssembly = Assembly.GetExecutingAssembly();
		AssemblyLoadContext context = AssemblyLoadContext.GetLoadContext(currentAssembly);
		try
		{
			Log.Info("Загрузка и запуск патчей...");
			var asm = context?.LoadFromAssemblyPath(localPatchInfo.AbsolutePath);
			PatchManager.LoadPatches(asm);
		}
		catch (Exception ex)
		{
			Log.Warning($"Не удалось загрузить патчи из {localPatchInfo.FilePath}. Существует ли файл?: {ex}");
		}
		PatchManager.RunPatches();
	}
	
	private static void SaveSettings()
	{
		if (EventBus.PublishIsCancelled(new SettingsSavingEvent(Settings)))
		{
			return;
		}
		
		SettingsUtils.SaveSettings(Settings);
	}

	private void SetupUi()
	{
		PlayerNameTextBox.TextChanged += UpdatePlayerName;
		PlayerNameTextBox.Text = Settings.PlayerName;
		
		PasswordTextBox.TextChanged += UpdatePassword;
		PasswordTextBox.Text = Settings.Password;
		
		RamSlider.MinValue = 1;
		RamSlider.MaxValue = GetInstalledRamAmount();
		RamSlider.ValueChanged += UpdateRam;
		RamSlider.Value = Settings.MaxRam;
		RamSlider.Step = 0.5;

		RunButton.Pressed += DeployAndRun;

		ModsButton.Pressed += OpenModsMenu;
	}

	private void DeployAndRun()
	{
		if (EventBus.PublishIsCancelled(new RunButtonPressedEvent(RunButton)))
		{
			Log.Warning("Запуск игры был отменен одним из обработчиков события");
			return;
		}
			
		if (!State.CanLaunch)
		{
			Log.Error("Попытка запустить игру, когда она уже запущена или еще не готова к запуску");
			return;
		}

		State.IsMinecraftRunning = true;

		var filesystemTask = new PrepareFilesystemTask();
		var requiredModsTask = new CheckRequiredModsTask().AfterTasks(filesystemTask);
		var optionalModsTask = new CheckOptionalModsTask().AfterTasks(filesystemTask);
		var updateServers = new UpdateServersTask();
		var deployMods = new DeployModpackTask()
			.AfterTasks(requiredModsTask, optionalModsTask);
		var run = new RunMinecraftTask()
			.AfterTasks(deployMods, updateServers)
			.SkipIf(() => State.RunInterruptRequested);

		LauncherTask[] gameLaunchTaskSet =
			[filesystemTask, requiredModsTask, optionalModsTask, deployMods, updateServers, run];
		var evt = new GameAboutToRunEvent(gameLaunchTaskSet);
		if (EventBus.PublishIsCancelled(evt))
		{
			return;
		}

		gameLaunchTaskSet = evt.TaskSet;
		
		TaskManager.AddTasks(gameLaunchTaskSet);
	}

	private void UpdatePlayerName(string name)
	{
		if (EventBus.PublishIsCancelled(new PlayerNameUpdatingEvent(PlayerNameTextBox)))
			return;
		
		Settings.PlayerName = name;
		SaveSettings();
	}
	
	private void UpdatePassword(string name)
	{
		if (EventBus.PublishIsCancelled(new PlayerPasswordUpdatingEvent(PasswordTextBox)))
			return;
		
		Settings.PlayerName = name;
		SaveSettings();
	}
	
	private void UpdateRam(double value)
	{
		var evt = new RamValueChangingEvent(value);
		if (EventBus.PublishIsCancelled(evt))
		{
			//RamSlider.SetValueNoSignal(value);
			return;
		}

		value = evt.AmountGb;
		Settings.MaxRam = value;
		RamLabel.Text = $"RAM: {value:N1} / {GetInstalledRamAmount():N1} Gb";
		SaveSettings();
	}
	
	private void OpenModsMenu()
	{
		OptionalModsMenu.ShowMenu();
	}
	
	private static void InitNbtSystems()
	{
		// create NBT and fill it with random data. 
		var tag = new TagCompound();
		tag["version"] = 1;
		tag["vector"] = Vec(100, -3);
        
		// Then read it back
		var vec = tag.Get<Vector2>("vector");
	}
}