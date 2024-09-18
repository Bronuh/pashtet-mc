using System.IO;
using System.Threading.Tasks;
using BronuhMcBackend.Utils;
using Launcher;

namespace Tasks.Implementations;

public class PrepareEnvironmentTask : LauncherTask
{
    public override string Name { get; } = "Подготовка окружения игры";
    protected override async Task Start()
    {
        Directory.CreateDirectory(Paths.DownloadsDirPath.AsAbsolute());
        Directory.CreateDirectory(Paths.JreDirPath.AsAbsolute());
        
        Directory.CreateDirectory(Paths.SnapshotsDirPath.AsAbsolute());
        Directory.CreateDirectory(Paths.SnapshotModsDirPath.AsAbsolute());
        Directory.CreateDirectory(Paths.SnapshotConfigDirPath.AsAbsolute());
        
        Directory.CreateDirectory(Paths.MinecraftDirPath.AsAbsolute());
        Directory.CreateDirectory(Paths.MinecraftConfigDirPath.AsAbsolute());
        Directory.CreateDirectory(Paths.MinecraftModsDirPath.AsAbsolute());
        
        Directory.CreateDirectory(Paths.UserFilesDirPath.AsAbsolute());
        Directory.CreateDirectory(Paths.UserModsDirPath.AsAbsolute());
        Directory.CreateDirectory(Paths.UserConfigDirPath.AsAbsolute());
    }

    public override IEnumerable<LauncherTask> GetNextTasks()
    {
        var jreTask = new PrepareJreTask();
        var minecraftTask = new PrepareMinecraftTask();
        var modsTask = new CheckModsTask().AfterTasks(minecraftTask);
        var finishTask = new FinishPreparationsTask().AfterTasks(jreTask, minecraftTask, modsTask);
        
        return [jreTask, minecraftTask, modsTask, finishTask];
    }
}