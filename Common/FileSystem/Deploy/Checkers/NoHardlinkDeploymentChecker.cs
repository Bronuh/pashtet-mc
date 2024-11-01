namespace Common.FileSystem.Deploy;

public class NoHardlinkDeploymentChecker : IHardLinkDeploymentChecker
{
    public bool IsHardLinkDeploymentAvailable(string path)
    {
        return false;
    }

    public bool IsHardLinkDeploymentAvailable(string from, string to)
    {
        return false;
    }
}