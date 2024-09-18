using System.IO;
using System.Threading.Tasks;
using BronuhMcBackend.Utils;
using IO;
using Launcher;

namespace Tasks.Implementations;

public class DownloadMinecraftTask : LauncherTask
{
    public override string Name { get; } = "Скачивание Minecraft";
    public override double Progress => _dlTask?.GetProgress() ?? 0;
    
    private DownloadTask _dlTask;
    
    protected override async Task Start()
    {
        var minecraftUrl = Urls.MinecraftUrl;
        var targetZip = Paths.MinecraftZipPath.AsAbsolute();
        
        if (File.Exists(targetZip))
            return;
        
        _dlTask = new DownloadTask(minecraftUrl, targetZip);
        await _dlTask.RunAsync();
    }

    public override IEnumerable<LauncherTask> GetNextTasks()
    {
        return [new UnpackMinecraftTask()];
    }
}