namespace Common.Api;

public class ModInfo(string readableName, string fileName, string description, Dictionary<string, object> metadata = null)
{
    public string FileName { get; set; } = fileName;
    public Dictionary<string, object> Metadata { get; } = metadata;
    public string ReadableName { get; } = readableName;
    public string Description { get; } = description;

    public override string ToString()
    {
        return $"{ReadableName}: {Description}";
    }
}