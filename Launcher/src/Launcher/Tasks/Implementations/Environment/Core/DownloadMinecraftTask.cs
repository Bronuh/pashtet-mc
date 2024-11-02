#region

using System.IO;
using System.Threading.Tasks;
using HashedFiles;
using IO;
using Launcher.Nodes;

#endregion

namespace Launcher.Tasks.Implementations.Environment.Core;

public class DownloadMinecraftTask : LauncherTask
{
    public override string Name { get; } = "Скачивание Minecraft";
    public override double Progress => _dlTask?.GetProgress() ?? 0;
    
    private DownloadTask _dlTask;
    
    protected override async Task Start()
    {
        var minecraftUrl = Main.ApiProvider.GetMinecraftUrl();
        var targetZip = Paths.MinecraftZipPath.AsAbsolute();
        
        if (File.Exists(targetZip))
            return;
        
        _dlTask = new DownloadTask(minecraftUrl, targetZip);
        await _dlTask.RunAsync();
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return [new UnpackMinecraftTask()];
    }
}