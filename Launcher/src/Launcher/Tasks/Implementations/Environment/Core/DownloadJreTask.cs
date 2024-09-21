using System.IO;
using System.Threading.Tasks;
using BronuhMcBackend.Utils;
using IO;
using Launcher;

namespace Tasks.Implementations;

public class DownloadJreTask : LauncherTask
{
    public override string Name { get; } = "Скачивание Java";
    public override double Progress => _dlTask?.GetProgress() ?? 0;
    
    private DownloadTask _dlTask;
    
    protected override async Task Start()
    {
        var jreUrl = Urls.JreUrl;
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