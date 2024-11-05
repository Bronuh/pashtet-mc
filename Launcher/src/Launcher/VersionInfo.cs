namespace Launcher;

public sealed class VersionInfo
{
    public Version CoreVersion { get; private set; }
    public string CoreVersionTag { get; private set; }
    
    public Version PatchVersion { get; set; }
    public string PatchVersionTag { get; set; }

    public VersionInfo(Version coreVersion, string coreVersionTag = null)
    {
        CoreVersion = coreVersion;
        CoreVersionTag = coreVersionTag;
    }
}