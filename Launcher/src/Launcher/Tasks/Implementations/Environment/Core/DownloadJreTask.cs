#region

using System.IO;
using System.Threading.Tasks;
using HashedFiles;
using IO;
using Launcher.Nodes;

#endregion

namespace Launcher.Tasks.Environment.Core;

public class DownloadJreTask : LauncherTask
{
    public override string Name { get; } = "Скачивание Java";
    public override double Progress => _dlTask?.GetProgress() ?? 0;
    
    private DownloadTask _dlTask;
    
    protected override async Task Start()
    {
        var jreUrl = Main.ApiProvider.GetJavaUrl();
        var targetZip = Paths.JreZipPath.AsAbsolute();
        
        if (File.Exists(targetZip))
            return;
        
        _dlTask = new DownloadTask(jreUrl, targetZip);
        await _dlTask.RunAsync();
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return [new UnpackJreTask()];
    }
}