#region

using System.IO;
using System.Threading.Tasks;
using HashedFiles;
using IO;

#endregion

namespace Launcher.Tasks.Implementations.Environment.Core;

public class UnpackJreTask : LauncherTask
{
    public override double Progress => _unpackTask?.GetProgress() ?? 0;
    public override string Name { get; } = "Распаковка Java";
    
    private UnpackTask _unpackTask;

    protected override async Task Start()
    {
        var zipPath = Paths.JreZipPath.AsAbsolute();
        var jrePath = Paths.JreDirPath.AsAbsolute();
        
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