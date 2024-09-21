using System.IO;
using System.Threading.Tasks;
using BronuhMcBackend.Utils;
using IO;
using Launcher;

namespace Tasks.Implementations;

public class BackupSnapshotTask : LauncherTask
{
    public override double Progress => _directoryCopyTask?.GetProgress() ?? 0;
    private DirectoryCopyTask _directoryCopyTask;
    public override string Name { get; } = "Сохранение бэкапа текущего модпака";
    protected override async Task Start()
    {
        var now = DateTime.Now;
        var backupName = $"Backup_{now:yyyyMMddHHmmssfff}";
        
        var sourcePath = Paths.CurrentSnapshotDirPath.AsAbsolute();
        var destinationPath = Path.Combine(Paths.SnapshotsDirPath.AsAbsolute(), backupName);
        
        _directoryCopyTask = new DirectoryCopyTask(sourcePath, destinationPath);
        await _directoryCopyTask.Run();
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return null;
    }
}