using Godot;
using System;
using System.Linq;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using KludgeBox.Scheduling;
using Launcher;
using Tasks.Implementations;
using Tasks.Implementations.Info;
using TaskState = Tasks.TaskState;

public partial class Main : Node
{
	public static Main Instance { get; private set; }
	public static TaskManager TaskManager { get; private set; }
	public static Scheduler Scheduler { get; private set; }
	public static Settings Settings { get; private set; }
	public static bool GameIsRunning { get; set; } = false;

	[Export] public LineEdit PlayerNameTextBox;
	[Export] public Label RunningTasksLabel;
	[Export] public Label PendingTasksLabel;
	[Export] public Label RamLabel;
	[Export] public Button RunButton;
	[Export] public HSlider RamSlider;
	[Export] public VBoxContainer RunningTasksContainer;
	[Export] public VBoxContainer PendingTasksContainer;
	[Export] public PanelContainer ConfigPanel;
	[Export] public PackedScene TaskTrackerScene;
	
	public override void _Ready()
	{
		Instance = this;
		Scheduler = new ();
		TaskManager = new TaskManager(Scheduler);
		Settings = SettingsUtils.LoadSettings();
		SetupUi();
		SettingsUtils.SaveSettings(Settings);

		var prepareTask = new PrepareEnvironmentTask();
		var jreTask = new PrepareJreTask().AfterTasks(prepareTask);
		var minecraftTask = new PrepareMinecraftTask().AfterTasks(prepareTask);
		var finishTask = new FinishPreparationsTask().AfterTasks(jreTask, minecraftTask);
		
		TaskManager.AddTasks([prepareTask, jreTask, minecraftTask, finishTask]);
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
			pendingTracker.From(pendingTask);
			PendingTasksContainer.AddChild(pendingTracker);
		}
		
		foreach (var runningTask in missingRunningTasks)
		{
			var runningTracker = TaskTrackerScene.Instantiate() as TaskTracker;
			runningTracker.From(runningTask);
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

	private static void SaveSettings()
	{
		SettingsUtils.SaveSettings(Settings);
	}

	private void SetupUi()
	{
		PlayerNameTextBox.TextChanged += UpdatePlayerName;
		PlayerNameTextBox.Text = Settings.PlayerName;
		
		RamSlider.MinValue = 1;
		RamSlider.MaxValue = GetInstalledRamAmount();
		RamSlider.ValueChanged += UpdateRam;
		RamSlider.Value = Settings.MaxRam;
		RamSlider.Step = 0.5;

		RunButton.Pressed += DeployAndRun;
	}

	private void DeployAndRun()
	{
		if (GameIsRunning)
		{
			Log.Error("Game is already running");
			return;
		}

		GameIsRunning = true;
		
		var modsTask = new CheckModsTask();
		var deployMods = new DeployModpackTask().AfterTasks(modsTask);
		var updateServers = new UpdateServersTask();
		var deletePacks = new DeleteServerResourcepackTask();
		var run = new RunMinecraftTask().AfterTasks(deployMods, updateServers, deletePacks);
		
		TaskManager.AddTasks([modsTask, deployMods, updateServers, deletePacks, run]);
	}

	private void UpdatePlayerName(string name)
	{
		Settings.PlayerName = name;
		SaveSettings();
	}
	
	private void UpdateRam(double value)
	{
		Settings.MaxRam = value;
		RamLabel.Text = $"RAM: {value:N1} / {GetInstalledRamAmount():N1} Gb";
		SaveSettings();
	}
}
