#region

using System.IO;
using System.Threading.Tasks;
using HashedFiles;
using IO;
using Launcher.Nodes;

#endregion

namespace Launcher.Tasks.Environment.Core;

public class UnpackJreTask : LauncherTask
{
    public override double Progress => _unpackTask?.GetProgress() ?? 0;
    public override string Name { get; } = "Распаковка Java";
    
    private UnpackTask _unpackTask;

    protected override async Task Start()
    {
        var zipPath = Paths.JreZipPath.AsAbsolute();
        var jrePath = Paths.JreDirPath.AsAbsolute();

        if (!File.Exists(zipPath))
            throw new FileNotFoundException($"Java zip not found: {zipPath}");
        
        _unpackTask = new UnpackTask(zipPath, jrePath);
        
        await _unpackTask.RunAsync();
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        Main.State.IsJavaReady = true;
        return null;
    }
}