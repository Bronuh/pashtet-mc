using Godot;
using System;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;

public partial class Bootstrapper : Node
{
	[Export] private Button StartButton; 
	public override void _Ready()
	{
		StartButton.Pressed += Start;
	}

	private async void Start()
	{
		var path = new MinecraftPath("./minecraft");
		path.Runtime = "./jre";
		
		var launcher = new MinecraftLauncher(path);
		
		// add event handlers
		launcher.FileProgressChanged += (sender, args) =>
		{
			GD.Print($"Name: {args.Name}");
			GD.Print($"Type: {args.EventType}");
			GD.Print($"Total: {args.TotalTasks}");
			GD.Print($"Progressed: {args.ProgressedTasks}");
		};
		launcher.ByteProgressChanged += (sender, args) =>
		{
			GD.Print($"{args.ProgressedBytes} bytes / {args.TotalBytes} bytes");
		};
		
		var process = await launcher.BuildProcessAsync("Forge 1.20.1", new MLaunchOption
		{
			Session = MSession.CreateOfflineSession("Bronuh"),
			MaximumRamMb = 4096
		});
		process.Start();
	}
}
