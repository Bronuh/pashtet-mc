#region

using System.IO;
using System.Threading.Tasks;
using HashedFiles;
using IO;

#endregion

namespace Launcher.Tasks.Implementations.Environment.Core;

public class UnpackMinecraftTask : LauncherTask
{
    public override double Progress => _unpackTask?.GetProgress() ?? 0;
    
    private UnpackTask _unpackTask;
    public override string Name { get; } = "Распаковка Minecraft";

    protected override async Task Start()
    {
        var zipPath = Paths.MinecraftZipPath.AsAbsolute();
        var jrePath = Paths.MinecraftDirPath.AsAbsolute();
        
        if(!File.Exists(zipPath))
            return;
        
        _unpackTask = new UnpackTask(zipPath, jrePath);
        
        await _unpackTask.RunAsync();
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return null;
    }
}