#region

using System.Threading.Tasks;
using HashedFiles;
using Launcher.Nodes;

#endregion

namespace Launcher.Tasks.Launch;

public class RunMinecraftTask : LauncherTask
{
    public override string Name { get; } = "Запуск Minecraft";

    public RunMinecraftTask()
    {
        TakingSlot = false;
        IsVisible = false;
    }
    
    protected override async Task Start()
    {
        var minecraftPath = Paths.MinecraftDirPath.AsAbsolute();
        var jrePath = Paths.JreExecutablePath.AsAbsolute();
        var maxRam = Main.Settings.MaxRam;
        var playerName = Main.Settings.PlayerName;
        var scheduler = Main.Scheduler;
        
        var launcher = new BhLauncher(minecraftPath, jrePath, scheduler, playerName, maxRam);

        await launcher.Run();
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        Main.State.IsMinecraftRunning = false;
        Main.State.RunInterruptRequested = false;
        return null;
    }
}