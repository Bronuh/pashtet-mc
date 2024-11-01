namespace Common.IO.Checksum;

public interface IChecksumProvider
{
    string CalculateChecksum(Stream stream);
}