#region

using System.IO;
using System.Threading.Tasks;
using HashedFiles;
using Launcher.Nodes;

#endregion

namespace Launcher.Tasks.Implementations.Environment.Core;

public class PrepareMinecraftTask : LauncherTask
{
    private LauncherTask _nextTask;
    public override string Name { get; } = "Подготвка Minecraft";
    protected override async Task Start()
    {
        var minecraftPath = Paths.MinecraftDirPath.AsAbsolute();
        var minecraftVersionPath = Path.Combine(minecraftPath, "versions/Forge 1.20.1/Forge 1.20.1.jar");
        
        if (File.Exists(minecraftVersionPath))
        {
            Main.State.IsMinecraftReady = true;
            return;
        }
        
        if (File.Exists(Paths.MinecraftZipPath.AsAbsolute()))
        {
            _nextTask = new UnpackMinecraftTask();
            return;
        }
        
        _nextTask = new DownloadMinecraftTask();
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return [_nextTask];
    }
}