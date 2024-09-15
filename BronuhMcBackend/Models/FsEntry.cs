using System.Text.Json.Serialization;

namespace BronuhMcBackend.Utils;

public abstract class FsEntry
{
    public string Name => Path.GetFileName(RelativePath);
    public string RelativePath { get; private set; }
    public bool IsLoose { get; private set; }
    
    [JsonIgnore]
    public string AbsolutePath { get; private set; }
    [JsonIgnore]
    public string RootPath { get; private set; }

    public FsEntry(string path, string rootPath, bool loose = false)
    {
        AbsolutePath = path;
        RelativePath = Path.GetRelativePath(rootPath, path);
        RootPath = rootPath;
        IsLoose = loose;
    }
}