using System.IO;

namespace BronuhMcBackend.Utils;

public class MinimalDirectoryEntry : FsEntry
{
    public MinimalDirectoryEntry(string path, string rootPath, bool loose = false) : base(path, rootPath, loose)
    {
        if(!Directory.Exists(path))
            throw new DirectoryNotFoundException(path);
    }
}