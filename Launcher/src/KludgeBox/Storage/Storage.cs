#region

using KludgeBox.VFS;
using KludgeBox.VFS.Base;
using KludgeBox.VFS.FileSystems;

#endregion

namespace KludgeBox.Storage;

public static class Storage
{
    //private static Dictionary<string, FsFile> _filesMap = new();
    //private static GodotFileSystem _builtinFs = new GodotFileSystem(GodotFsRoot.Res);
    private static PhysicalFileSystem _defaultFs = new("./Content");
    private static GodotFileSystem _userFs = new GodotFileSystem(GodotFsRoot.User, "/ResourcesOverride", true);
    private static MergedFileSystem _mergedFs = new MergedFileSystem();

    public static void Init()
    {
        //ScanDir(_builtinFs.Root);
        //ScanDir(_userFs.Root);

        //_mergedFs.AddFileSystem(_builtinFs);
        _mergedFs.AddFileSystem(_defaultFs);
        _mergedFs.AddFileSystem(_userFs);
    }
    
    /*public static void AddFile(string path, FsFile file)
    {
        _filesMap[path.NormalizePath()] = file;
    }*/
    
    /*public static void AddDirectory(FsDirectory dir)
    {
        ScanDir(dir);
    }*/
    
    public static FsFile GetFile(string path)
    {
        /*if (_filesMap.TryGetValue(path.NormalizePath(), out var file))
        {
            return file;
        }

        throw new FileNotFoundException(path.NormalizePath());*/

        return _mergedFs.GetFile(path.NormalizePath());
    }
    
    public static FsDirectory GetDirectory(string path)
    {
        return _mergedFs.GetDirectory(path.NormalizePath());
    }

    /*private static void ScanDir(FsDirectory dir)
    {
        var files = dir.Files;
        foreach (var file in files)
        {
            AddFile(file.Path, file);
        }
        
        var dirs = dir.Directories;
        foreach (var subDir in dirs)
        {
            ScanDir(subDir);
        }
    }*/
}