using Godot;
using System;
using BronuhMcBackend.Utils;
using KludgeBox.VFS;
using Launcher;
using Tasks.Implementations;

public partial class ConfigContainer : PanelContainer
{
	[Export] public Control RootControl;
	[Export] public Button ConfigButton;
	[Export] public Button OpenWorkingDirButton;
	[Export] public Button OpenUserModsDirButton;
	[Export] public Button ResetConfigButton; 

	public float Width => Size.X;
	public float RootWidth => RootControl.Size.X;
	
	public float Height => Size.Y;
	public float RootHeight => RootControl.Size.Y;
	public float OffsetX => Position.X;
	public float RootRightX => RootControl.Size.X;

	public double TweenDuration = 0.25;

	private bool _doHide = true;
	public override void _Ready()
	{
		ConfigButton.Toggled += OnToggled;
		OpenWorkingDirButton.Pressed += () => OpenDir(OS.GetUserDataDir());
		OpenUserModsDirButton.Pressed += () => OpenDir(Paths.UserModsDirPath.AsAbsolute());
		ResetConfigButton.Pressed += () =>
		{
			var configDl = new DownloadConfigsTask();
			var configUnpack = new UnpackConfigsTask().AfterTasks(configDl);
			Main.TaskManager.AddTasks([configDl, configUnpack]);
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		DoHide();
	}

	private void OpenDir(string path)
	{
		path = path.NormalizePath();
		Log.Info($"Opening directory: {path}");
		OS.ShellOpen(path);
	}

	private void DoHide()
	{
		if (_doHide)
		{
			Position = RootControl.Position + Vec(RootRightX, 0);
		}
	}

	private void OnToggled(bool value)
	{
		if (value)
		{
			_doHide = false;
			Position = RootControl.Position + Vec(RootRightX, 0);
			var tween = CreateTween();
			tween.TweenProperty(this, "position:x", RootRightX - Width, TweenDuration)
				.SetEase(Tween.EaseType.Out)
				.SetTrans(Tween.TransitionType.Cubic);
			tween.Play();
		}
		else
		{
			Position = RootControl.Position + Vec(RootRightX - Width, 0);
			var tween = CreateTween();
			tween.TweenProperty(this, "position:x", RootRightX, TweenDuration)
				.SetEase(Tween.EaseType.Out)
				.SetTrans(Tween.TransitionType.Cubic);
			tween.TweenCallback(Callable.From(() => _doHide = true));
			tween.Play();
		}
	}
}
