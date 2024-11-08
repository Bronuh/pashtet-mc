namespace Common.Api;

public class ModInfo(string readableName, string fileName, string description, Dictionary<string, string> metadata = null)
{
    public static string MetaModUrl = "modUrl";
    public static string MetaModVersion = "modVersion";
    public static string MetaModDownloadUrl = "downloadUrl";
    public static string MetaModDependencies = "dependsOn";
    
    public string ReadableName { get; } = readableName;
    public string FileName { get; set; } = fileName;
    public string Description { get; } = description;
    public Dictionary<string, string> Metadata { get; } = metadata;

    public override string ToString()
    {
        return $"{ReadableName}: {Description}";
    }
}