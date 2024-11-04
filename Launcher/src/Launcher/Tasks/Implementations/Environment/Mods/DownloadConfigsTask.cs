#region

using System.IO;
using System.Threading.Tasks;
using HashedFiles;
using IO;

#endregion

namespace Launcher.Tasks.Environment.Mods;

public class DownloadConfigsTask : LauncherTask
{
    public override string Name => "Обновление настроек";
    public override double Progress => _dlTask?.GetProgress() ?? 0;
    
    private DownloadTask _dlTask;
    protected override async Task Start()
    {
        var configsUrl = Urls.FileSnapshotDownloadUrl + "config/config.zip";
        var targetZip = Path.Combine(Paths.DownloadsDirPath.AsAbsolute(),"config.zip");
        
        if (File.Exists(targetZip))
            File.Delete(targetZip);
        
        _dlTask = new DownloadTask(configsUrl, targetZip);
        await _dlTask.RunAsync();
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return null;
    }
}