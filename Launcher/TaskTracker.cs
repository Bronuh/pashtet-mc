using Godot;
using System;
using Tasks;

public partial class TaskTracker : PanelContainer
{
	[Export] public Label TaskNameLabel { get; set; }
	[Export] public ProgressBar ProgressBar { get; set; }
	
	public LauncherTask TrackingTask { get; private set; }

	public override void _Process(double delta)
	{
		TaskNameLabel.Text = TrackingTask.Name;
		ProgressBar.Value = TrackingTask.Progress * 100;
	}

	public void From(LauncherTask task)
	{
		TrackingTask = task;
	}
}
