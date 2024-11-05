#region

using System.Threading.Tasks;
using HashedFiles;
using KludgeBox.Events.Global;
using Launcher.Nodes;
using PatchApi.Events;

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

        var evt = new MinecraftStartingEvent(minecraftPath, jrePath, maxRam, playerName, scheduler);
        
        if(EventBus.PublishIsCancelled(evt))
            return;
        
        var launcher = new BhLauncher(evt.MinecraftPath, evt.JavaPath, evt.Scheduler, evt.PlayerName, evt.MaxRam);

        await launcher.Run();
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        Main.State.IsMinecraftRunning = false;
        Main.State.RunInterruptRequested = false;
        return null;
    }
}