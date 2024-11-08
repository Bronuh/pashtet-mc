#region

using System.IO;

#endregion

namespace Launcher;

public static class Paths
{
    public static string DownloadsDirName = "downloads";
    public static string CoreDirName = "core";
    public static string JreDirName = "jre";
    public static string JreFileName = "jdk-21.zip";
    public static string JreExecutableName = "javaw.exe";
    public static string MinecraftDirName = "minecraft";
    public static string MinecraftFileName = "minecraft-min.zip";
    
    public static string SnapshotsDirName = "snapshots";
    public static string CurrentDirName = "current";
    public static string ModsDirName = "mods";
    public static string ConfigDirName = "config";
    public static string OptionalModsDirName = "optional-mods";
    
    public static string LauncherDirName = "launcher";
    public static string NewsFileName = "news.txt";
    public static string PatchFileName = "LauncherPatches.dll";
    
    public static string UserFilesDirName = "user";
    public static string CustomDataFileName = "custom-data.nbt";
    
    public static string CoreDirPath => CoreDirName;
    public static string DownloadsDirPath => DownloadsDirName;
    public static string JreDirPath => Path.Combine(CoreDirPath, JreDirName);
    public static string JreZipPath => Path.Combine(DownloadsDirName, JreFileName);
    public static string JreExecutablePath => Path.Combine(JreDirPath, "bin", JreExecutableName);
    public static string MinecraftDirPath => Path.Combine(CoreDirPath, MinecraftDirName);
    public static string MinecraftZipPath => Path.Combine(DownloadsDirName, MinecraftFileName);
    public static string MinecraftModsDirPath => Path.Combine(MinecraftDirPath, ModsDirName);
    public static string MinecraftConfigDirPath => Path.Combine(MinecraftDirPath, ConfigDirName);
    
    public static string SnapshotsDirPath => SnapshotsDirName;
    public static string CurrentSnapshotDirPath => Path.Combine(SnapshotsDirPath, CurrentDirName);
    public static string SnapshotModsDirPath => Path.Combine(CurrentSnapshotDirPath, ModsDirName);
    public static string SnapshotOptionalModsDirPath => Path.Combine(CurrentSnapshotDirPath, OptionalModsDirName);
    public static string SnapshotConfigDirPath => Path.Combine(CurrentSnapshotDirPath, ConfigDirName);
    
    public static string UserFilesDirPath => UserFilesDirName;
    public static string UserModsDirPath => Path.Combine(UserFilesDirPath, ModsDirName);
    public static string UserConfigDirPath => Path.Combine(UserFilesDirPath, ConfigDirName);
    
    public static string LauncherDirPath => LauncherDirName;
    public static string NewsFilePath => NewsFileName;
    public static string PatchFilePath => PatchFileName;
    public static string CustomDataFilePath => CustomDataFileName;
    
    
    public static string RootPath => ProjectSettings.GlobalizePath("user://");
}