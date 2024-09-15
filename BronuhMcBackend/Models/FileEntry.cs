using System.IO.Hashing;

namespace BronuhMcBackend.Utils;

public class FileEntry : FsEntry
{
    public string Checksum { get; private set; }
    public FileEntry(string path, string rootPath, bool loose = false) : base(path, rootPath, loose)
    {
        if(!File.Exists(path))
            throw new FileNotFoundException(path);
        
        Checksum = loose ? "" : CalculateFileChecksum();
    }
    
    private string CalculateFileChecksum()
    {
        using (var stream = System.IO.File.OpenRead(AbsolutePath))
        {
            var hash = new XxHash3();
            hash.Append(stream);
            var str = BitConverter.ToString(hash.GetCurrentHash()).Replace("-", "").ToLower();
            return str;
        }
    }
}