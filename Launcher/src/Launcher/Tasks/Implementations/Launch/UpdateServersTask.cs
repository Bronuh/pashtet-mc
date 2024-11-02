#region

using System.IO;
using System.Threading.Tasks;
using HashedFiles;
using IO;
using Launcher.Nodes;

#endregion

namespace Launcher.Tasks.Implementations.Launch;

public class UpdateServersTask : LauncherTask
{
    public override string Name => "Обновление списка серверов";
    public override double Progress => _dlTask?.GetProgress() ?? 0;
    
    private DownloadTask _dlTask;
    protected override async Task Start()
    {
        var serversUrl = Main.ApiProvider.GetServersUrl();
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