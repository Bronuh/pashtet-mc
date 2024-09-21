using System.IO;
using System.Threading.Tasks;
using BronuhMcBackend.Utils;
using IO;
using Launcher;

namespace Tasks.Implementations;

public class UpdateServersTask : LauncherTask
{
    public override string Name => "Обновление списка серверов";
    public override double Progress => _dlTask?.GetProgress() ?? 0;
    
    private DownloadTask _dlTask;
    protected override async Task Start()
    {
        var serversUrl = Urls.FileSnapshotDownloadUrl + "/servers.dat";
        var targetFile = Path.Combine(Paths.MinecraftDirPath.AsAbsolute(), "servers.dat");
        
        if (File.Exists(targetFile))
            File.Delete(targetFile);
        
        _dlTask = new DownloadTask(serversUrl, targetFile);
        await _dlTask.RunAsync();
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return null;
    }
}