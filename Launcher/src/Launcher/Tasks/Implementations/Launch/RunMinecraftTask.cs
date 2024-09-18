using System.Threading.Tasks;
using BronuhMcBackend.Utils;
using Launcher;

namespace Tasks.Implementations;

public class RunMinecraftTask : LauncherTask
{
    public override string Name { get; } = "Запуск Minecraft";
    
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

    public override IEnumerable<LauncherTask> GetNextTasks()
    {
        Main.GameIsRunning = false;
        return null;
    }
}