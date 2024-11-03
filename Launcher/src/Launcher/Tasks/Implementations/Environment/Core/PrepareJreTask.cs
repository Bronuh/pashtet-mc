#region

using System.IO;
using System.Threading.Tasks;
using HashedFiles;
using Launcher.Nodes;

#endregion

namespace Launcher.Tasks.Implementations.Environment.Core;

public class PrepareJreTask : LauncherTask
{
    private LauncherTask _nextTask;
    public override string Name { get; } = "Подготовка Java";
    
    protected override async Task Start()
    {
        if (File.Exists(Paths.JreExecutablePath.AsAbsolute()))
        {
            Main.State.IsJavaReady = true;
            return;
        }

        if (File.Exists(Paths.JreFileName.AsAbsolute()))
        {
            _nextTask = new UnpackJreTask();
            return;
        }

        _nextTask = new DownloadJreTask();
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return [_nextTask];
    }
}