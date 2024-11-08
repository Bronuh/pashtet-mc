#region

using System.IO;
using System.Threading.Tasks;
using HashedFiles;

#endregion

namespace Launcher.Tasks.Environment;

public class PrepareFilesystemTask : LauncherTask
{
    public override string Name { get; } = "Подготовка файловой системы игры";
    protected override async Task Start()
    {
        Directory.CreateDirectory(Paths.DownloadsDirPath.AsAbsolute());
        Directory.CreateDirectory(Paths.JreDirPath.AsAbsolute());
        
        Directory.CreateDirectory(Paths.SnapshotsDirPath.AsAbsolute());
        Directory.CreateDirectory(Paths.SnapshotModsDirPath.AsAbsolute());
        Directory.CreateDirectory(Paths.SnapshotOptionalModsDirPath.AsAbsolute());
        Directory.CreateDirectory(Paths.SnapshotConfigDirPath.AsAbsolute());
        
        Directory.CreateDirectory(Paths.MinecraftDirPath.AsAbsolute());
        Directory.CreateDirectory(Paths.MinecraftConfigDirPath.AsAbsolute());
        Directory.CreateDirectory(Paths.MinecraftModsDirPath.AsAbsolute());
        
        Directory.CreateDirectory(Paths.UserFilesDirPath.AsAbsolute());
        Directory.CreateDirectory(Paths.UserModsDirPath.AsAbsolute());
        Directory.CreateDirectory(Paths.UserConfigDirPath.AsAbsolute());
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return null;
    }
}