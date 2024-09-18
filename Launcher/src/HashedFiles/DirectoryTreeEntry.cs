using System.IO;
using System.Linq;

namespace BronuhMcBackend.Utils;

public sealed class DirectoryTreeEntry : DirectoryEntry
{
    public DirectoryTreeEntry(string path, string rootPath, bool loose = false) : base(path, rootPath, loose)
    {
        
    }
    
    protected override void FillEntries()
    {
        var dirs = Directory.GetDirectories(AbsolutePath).Select(path => new DirectoryEntry(path, RootPath, IsLoose));
        var files = Directory.GetFiles(AbsolutePath).Select(path => new FileEntry(path, RootPath, IsLoose));

        var list = new List<FsEntry>();

        list.AddRange(dirs);
        list.AddRange(files);
        
        Entries = list.ToArray();
    }
}