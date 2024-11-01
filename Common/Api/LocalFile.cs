using Common.IO.Checksum;

namespace Common.Api;

public record LocalFile
{
    public string AbsolutePath => Path.GetFullPath(FilePath);

    public string FileName { get; }

    public LocalFile(string filePath, string? fileName = null)
    {
        FilePath = filePath;
        FileName = fileName ?? Path.GetFileName(FilePath);
    }

    

    public string FilePath { get; init; }

    public Stream GetStream() => File.OpenRead(FilePath);
    public bool Exists() => File.Exists(AbsolutePath);
    
    public string GetChecksum(IChecksumProvider checksumProvider)
    {
        using var stream = File.OpenRead(FilePath);
        return checksumProvider.CalculateChecksum(stream);
    }

    public RemoteFile AsRemote(string url, IChecksumProvider checksumProvider)
    {
        var name = FileName ?? Path.GetFileName(FilePath);
        var checksum = GetChecksum(checksumProvider);
        
        return new RemoteFile(url, name, checksum);
    }
    
    public byte[] GetBytes() => File.ReadAllBytes(AbsolutePath);

    public void Deconstruct(out string FilePath, out string? FileName)
    {
        FilePath = this.FilePath;
        FileName = this.FileName;
    }
}