namespace BronuhMcBackend.Utils;

public class DirectoryEntry : MinimalDirectoryEntry
{
    public FsEntry[] Entries;
    public DirectoryEntry(string path, string rootPath, bool loose = false) : base(path, rootPath, loose)
    {
        // We have all needed data here
        // ReSharper disable once VirtualMemberCallInConstructor
        FillEntries();
    }

    protected virtual void FillEntries()
    {
        var dirs = Directory.GetDirectories(AbsolutePath).Select(path => new MinimalDirectoryEntry(path, RootPath, IsLoose));
        var files = Directory.GetFiles(AbsolutePath).Select(path => new FileEntry(path, RootPath, IsLoose));

        var list = new List<FsEntry>();

        list.AddRange(dirs);
        list.AddRange(files);
        
        Entries = list.ToArray();
    }

    public IEnumerable<FileEntry> GetFiles()
    {
        return Entries.OfType<FileEntry>();
    }
    
    public IEnumerable<DirectoryEntry> GetDirectories()
    {
        return Entries.OfType<DirectoryEntry>();
    }
}