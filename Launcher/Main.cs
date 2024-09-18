using Godot;
using System;
using System.Linq;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using KludgeBox.Scheduling;
using Launcher;
using Tasks.Implementations;
using TaskState = Tasks.TaskState;

public partial class Main : Node
{
	public static Main Instance { get; private set; }
	public static TaskManager TaskManager { get; private set; }
	public static Scheduler Scheduler { get; private set; }
	public static Settings Settings { get; private set; }
	public static bool GameIsRunning { get; set; } = false;

	[Export] public LineEdit PlayerNameTextBox;
	[Export] public HSlider RamSlider;
	[Export] public Button RunButton;
	[Export] public Label RamLabel;
	[Export] public PackedScene TaskTrackerScene;

	[Export] public VBoxContainer RunningTasksContainer;
	[Export] public VBoxContainer PendingTasksContainer;
	
	public override void _Ready()
	{
		Instance = this;
		Scheduler = new ();
		TaskManager = new TaskManager(Scheduler);
		Settings = SettingsUtils.LoadSettings();
		SetupUi();
		SettingsUtils.SaveSettings(Settings);

		var prepareTask = new PrepareEnvironmentTask();
		TaskManager.AddTask(prepareTask);
	}
	
	public override void _Process(double delta)
	{
		Scheduler.Update(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		var shownRunningTasks = RunningTasksContainer.GetChildren().OfType<TaskTracker>().ToList();
		var shownPendingTasks = PendingTasksContainer.GetChildren().OfType<TaskTracker>().ToList();

		var missingRunningTasks = TaskManager.RunningTasks.Except(shownRunningTasks.Select(tt => tt.TrackingTask));
		var missingPendingTasks = TaskManager.PendingTasks.Except(shownPendingTasks.Select(tt => tt.TrackingTask));

		var runningTasksToHide = shownRunningTasks.Where(tt => !TaskManager.RunningTasks.Contains(tt.TrackingTask));
		var pendingTasksToHide = shownPendingTasks.Where(tt => !TaskManager.PendingTasks.Contains(tt.TrackingTask));

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
		
		RamSlider.MinValue = 0;
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

		var deploy = new DeployModpackTask();
		var run = new RunMinecraftTask().AfterTasks(deploy);
		
		TaskManager.AddTasks([deploy, run]);
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
