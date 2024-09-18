using System.IO;
using System.Reflection.PortableExecutable;
using Launcher;

namespace BronuhMcBackend.Utils;

public static class FilesystemContext
{
    
    public static string RootPath => Paths.RootPath;


    public static string AsAbsolute(this string relativePath)
    {
        var path = Path.Combine(RootPath, relativePath);
        return path;
    }

    public static IEnumerable<FileEntry> GetFiles(string path = "", bool loose = true)
    {
        Log.Debug($"""GetFiles: {path}""");
        return new DirectoryEntry(path, RootPath, loose).GetFiles();
    }

    public static FileEntry GetFile(string path = "", bool loose = true)
    {
        return new FileEntry(path, RootPath, loose);
    }

    public static IEnumerable<DirectoryEntry> GetDirectories(string path = "", bool loose = true)
    {
        return new DirectoryEntry(path, RootPath, loose).GetDirectories();
    }

    public static DirectoryEntry GetDirectory(string path = "", bool loose = true)
    {
        return new DirectoryEntry(path, RootPath, loose);
    }

    public static DirectoryTreeEntry GetTree(string path, string rootPath, bool loose = true)
    {
        return new DirectoryTreeEntry(path, rootPath, loose);
    }
}