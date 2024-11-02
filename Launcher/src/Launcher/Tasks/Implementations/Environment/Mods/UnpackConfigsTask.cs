#region

using System.IO;
using System.Threading.Tasks;
using HashedFiles;
using IO;

#endregion

namespace Launcher.Tasks.Implementations.Environment.Mods;

public class UnpackConfigsTask : LauncherTask
{
    
    public override double Progress => _unpackTask?.GetProgress() ?? 0;
    public override string Name { get; } = "Распаковка настроек";
    
    private UnpackTask _unpackTask;

    protected override async Task Start()
    {
        var targetZip = Path.Combine(Paths.DownloadsDirPath.AsAbsolute(),"config.zip");
        var configsDirPath = Paths.MinecraftConfigDirPath.AsAbsolute();
        
        if(!File.Exists(targetZip))
            return;
        
        _unpackTask = new UnpackTask(targetZip, configsDirPath);
        
        await _unpackTask.RunAsync();
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return null;
    }
}