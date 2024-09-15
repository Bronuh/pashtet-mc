using BronuhMcBackend.Models;
using Microsoft.Extensions.Options;

namespace BronuhMcBackend.Utils;

public sealed class FilesystemContext
{
    public const string CoreDirName = "core";
    public const string JreDirName = "jre";
    public const string JreFileName = "jdk-21.zip";
    public const string MinecraftDirName = "minecraft";
    public const string MinecraftFileName = "minecraft-min.zip";
    
    public const string SnapshotsDirName = "snapshots";
    public const string CurrentDirName = "current";
    public const string ModsDirName = "mods";
    public const string ConfigDirName = "config";
    
    public const string LauncherDirName = "launcher";
    public const string NewsFileName = "news.txt";
    
    public string CoreDirPath => CoreDirName;
    public string JreDirPath => Path.Combine(CoreDirPath, JreDirName);
    public string JreFilePath => Path.Combine(JreDirPath, JreFileName);
    public string MinecraftDirPath => Path.Combine(CoreDirPath, MinecraftDirName);
    public string MinecraftFilePath => Path.Combine(MinecraftDirPath, MinecraftFileName);
    
    public string SnapshotsDirPath => SnapshotsDirName;
    public string CurrentSnapshotDirPath => Path.Combine(SnapshotsDirPath, CurrentDirName);
    public string ModsDirPath => Path.Combine(CurrentSnapshotDirPath, ModsDirName);
    public string ConfigDirPath => Path.Combine(CurrentSnapshotDirPath, ConfigDirName);
    
    public string LauncherDirPath => LauncherDirName;
    public string NewsFilePath => NewsFileName;
    
    public string RootPath { get; private set; }
    
    private ILogger _logger;

    public FilesystemContext(IOptions<DirectorySettings> options, ILogger logger)
    {
        _logger = logger;
        RootPath = options.Value.RootDirectory;
    }

    public string AsAbsolute(string relativePath)
    {
        var path = Path.Combine(RootPath, relativePath);
        return path;
    }

    public IEnumerable<FileEntry> GetFiles(string path = "", bool loose = true)
    {
        _logger.LogDebug($"""GetFiles: {path}""");
        return new DirectoryEntry(path, RootPath, loose).GetFiles();
    }

    public FileEntry GetFile(string path = "", bool loose = true)
    {
        return new FileEntry(path, RootPath, loose);
    }

    public IEnumerable<DirectoryEntry> GetDirectories(string path = "", bool loose = true)
    {
        return new DirectoryEntry(path, RootPath, loose).GetDirectories();
    }

    public DirectoryEntry GetDirectory(string path = "", bool loose = true)
    {
        return new DirectoryEntry(path, RootPath, loose);
    }

    public DirectoryTreeEntry GetTree(string path, string rootPath, bool loose = true)
    {
        return new DirectoryTreeEntry(path, rootPath, loose);
    }
}