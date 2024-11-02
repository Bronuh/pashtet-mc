#region

using Common.IO.Checksum;

#endregion

namespace Common.Api;

public record LocalFile
{
    public string AbsolutePath => Path.GetFullPath(FilePath);

    public string FileName { get; }
    
    private string _checksum;

    public LocalFile(string filePath, string? fileName = null)
    {
        FilePath = filePath;
        FileName = fileName ?? Path.GetFileName(FilePath);
    }

    public LocalFile(string filePath, IChecksumProvider checksumProvider, string? fileName = null) : this(filePath,
        fileName)
    {
        _checksum = CalculateChecksum(checksumProvider);
    }

    public string FilePath { get; init; }

    public Stream GetStream() => File.OpenRead(FilePath);
    public bool Exists() => File.Exists(AbsolutePath);
    
    public string CalculateChecksum(IChecksumProvider checksumProvider)
    {
        using var stream = File.OpenRead(FilePath);
        return checksumProvider.CalculateChecksum(stream);
    }
    
    public string GetPrecalculatedChecksum() => _checksum;

    public RemoteFile AsRemote(string url, IChecksumProvider checksumProvider)
    {
        var name = FileName ?? Path.GetFileName(FilePath);
        var checksum = CalculateChecksum(checksumProvider);
        
        return new RemoteFile(name, url, checksum);
    }
    
    public byte[] GetBytes() => File.ReadAllBytes(AbsolutePath);

    public void Deconstruct(out string FilePath, out string? FileName)
    {
        FilePath = this.FilePath;
        FileName = this.FileName;
    }
}