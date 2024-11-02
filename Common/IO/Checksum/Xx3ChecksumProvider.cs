#region

using System.IO.Hashing;

#endregion

namespace Common.IO.Checksum;

public class Xx3ChecksumProvider : IChecksumProvider
{
    public string CalculateChecksum(Stream stream)
    {
        var hash = new XxHash3();
        hash.Append(stream);
        var str = BitConverter.ToString(hash.GetCurrentHash()).Replace("-", "").ToLower();
        return str;
    }
}