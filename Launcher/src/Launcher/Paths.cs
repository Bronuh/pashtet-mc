using System.IO;

namespace Launcher;

public static class Paths
{
    public const string DownloadsDirName = "downloads";
    public const string CoreDirName = "core";
    public const string JreDirName = "jre";
    public const string JreFileName = "jdk-21.zip";
    public const string JreExecutableName = "javaw.exe";
    public const string MinecraftDirName = "minecraft";
    public const string MinecraftFileName = "minecraft-min.zip";
    
    public const string SnapshotsDirName = "snapshots";
    public const string CurrentDirName = "current";
    public const string ModsDirName = "mods";
    public const string ConfigDirName = "config";
    
    public const string LauncherDirName = "launcher";
    public const string NewsFileName = "news.txt";
    
    public static string CoreDirPath => CoreDirName;
    public static string DownloadsDirPath => DownloadsDirName;
    public static string JreDirPath => Path.Combine(CoreDirPath, JreDirName);
    public static string JreZipPath => Path.Combine(DownloadsDirName, JreFileName);
    public static string JreExecutablePath => Path.Combine(JreDirPath, "bin", JreExecutablePath);
    public static string MinecraftDirPath => Path.Combine(CoreDirPath, MinecraftDirName);
    public static string MinecraftZipPath => Path.Combine(DownloadsDirName, MinecraftFileName);
    public static string MinecraftModsDirPath => Path.Combine(MinecraftDirPath, ModsDirName);
    public static string MinecraftConfigDirPath => Path.Combine(MinecraftDirPath, ConfigDirName);
    
    public static string SnapshotsDirPath => SnapshotsDirName;
    public static string CurrentSnapshotDirPath => Path.Combine(SnapshotsDirPath, CurrentDirName);
    public static string SnapshotModsDirPath => Path.Combine(CurrentSnapshotDirPath, ModsDirName);
    public static string SnapshotConfigDirPath => Path.Combine(CurrentSnapshotDirPath, ConfigDirName);
    
    public static string LauncherDirPath => LauncherDirName;
    public static string NewsFilePath => NewsFileName;
    
    
    public static string RootPath => ProjectSettings.GlobalizePath("user://");
}