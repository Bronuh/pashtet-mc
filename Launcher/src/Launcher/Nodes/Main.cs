#region

using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Api;
using Common;
using Common.Api;
using Common.FileSystem.Deploy;
using Common.IO.Checksum;
using HashedFiles;
using IO;
using KludgeBox.Events.Global;
using KludgeBox.Scheduling;
using Launcher.Tasks;
using Launcher.Tasks.Implementations.Environment;
using Launcher.Tasks.Implementations.Environment.Core;
using Launcher.Tasks.Implementations.Environment.Mods;
using Launcher.Tasks.Implementations.Info;
using Launcher.Tasks.Implementations.Launch;
using PatchApi;
using PatchApi.Events;

#endregion

namespace Launcher.Nodes;

public partial class Main : Node
{
	public static Main Instance { get; private set; }
	public static IApiProvider ApiProvider { get; set; }
	public static IChecksumProvider ChecksumProvider { get; set; }
	public static IFileDeployer FileDeployer { get; set; }
	public static TaskManager TaskManager { get; private set; }
	public static Scheduler Scheduler { get; private set; }
	public static Settings Settings { get; private set; }
	public static LauncherState State { get; private set; }
	public static PatchManager PatchManager { get; private set; }

	[Export] public LineEdit PlayerNameTextBox;
	[Export] public LineEdit PasswordTextBox;
	[Export] public Label RunningTasksLabel;
	[Export] public Label PendingTasksLabel;
	[Export] public Label RamLabel;
	[Export] public Button RunButton;
	[Export] public HSlider RamSlider;
	[Export] public VBoxContainer RunningTasksContainer;
	[Export] public VBoxContainer PendingTasksContainer;
	[Export] public PanelContainer ConfigPanel;
	[Export] public PackedScene TaskTrackerScene;
	[Export] public Popup Popup;
	
	public override async void _Ready()
	{
		Instance = this;
		Settings = SettingsUtils.LoadSettings();
		PatchManager = new PatchManager();
		
		Scheduler = new Scheduler();
		TaskManager = new TaskManager(Scheduler);
		ApiProvider = new DefaultApiProvider();
		State = new LauncherState();
		
		ChecksumProvider = DefaultServices.ChecksumProvider;
		FileDeployer = DefaultServices.FileDeployer;
		
		SettingsUtils.SaveSettings(Settings);
		Scheduler.PeriodicInSeconds(0.1, UpdateButton);
		SetupUi();

		await LoadPatches();
		
		EventBus.Publish(new CreatingMainTasksEvent());
		var prepareTask = new PrepareEnvironmentTask();
		var serverCheckTask = new PingServerTask();
		var cleanupDownloadsTask = new CleanupBrokenDownloads().AfterTasks(prepareTask);
		var jreTask = new PrepareJreTask().AfterTasks(prepareTask, cleanupDownloadsTask);
		var minecraftTask = new PrepareMinecraftTask().AfterTasks(prepareTask, cleanupDownloadsTask);
		var finishTask = new FinishPreparationsTask().AfterTasks(jreTask, minecraftTask);

		LauncherTask[] preparedTasks = [serverCheckTask, prepareTask, cleanupDownloadsTask, jreTask, minecraftTask, finishTask];
		
		EventBus.Publish(new RunningMainTasksOnTaskManagerEvent(preparedTasks));
		TaskManager.AddTasks(preparedTasks);
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

	private async Task LoadPatches()
	{
		Log.Info("Установка патчей...");
		var remotePatchInfo = await ApiProvider.GetPatchInfoAsync();
		var localPatchInfo = new LocalFile(Paths.PatchFilePath.AsAbsolute(), ChecksumProvider);
		if (remotePatchInfo is not null && remotePatchInfo.Checksum != localPatchInfo.GetPrecalculatedChecksum())
		{
			Log.Info("Скачивается патч...");
			var download = new DownloadTask(remotePatchInfo.Url, localPatchInfo.FilePath);
			await download.RunAsync();
		}
		
		Assembly currentAssembly = Assembly.GetExecutingAssembly();
		AssemblyLoadContext context = AssemblyLoadContext.GetLoadContext(currentAssembly);
		try
		{
			Log.Info("Загрузка и запуск патчей...");
			var asm = context!.LoadFromAssemblyPath(localPatchInfo.FilePath);
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
		
		var modsTask = new CheckModsTask();
		var updateServers = new UpdateServersTask();
		//var deletePacks = new DeleteServerResourcepackTask();
		var deployMods = new DeployModpackTask().AfterTasks(modsTask);
		var run = new RunMinecraftTask().AfterTasks(modsTask, deployMods, updateServers);
		
		TaskManager.AddTasks([modsTask, deployMods, updateServers,run]);
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
		if (EventBus.PublishIsCancelled(new RamValueChangingEvent(ref value)))
		{
			RamSlider.SetValueNoSignal(value);
			return;
		}
		
		Settings.MaxRam = value;
		RamLabel.Text = $"RAM: {value:N1} / {GetInstalledRamAmount():N1} Gb";
		SaveSettings();
	}
}