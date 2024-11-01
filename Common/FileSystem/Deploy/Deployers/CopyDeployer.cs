namespace Common.FileSystem.Deploy;


/// <summary>
/// This deployer perform deployment just by copying files.
/// </summary>
public class CopyDeployer : IFileDeployer
{
    public bool IsDeployable(string from, string to)
    {
        if (!File.Exists(from))
        {
            return false;
        }

        if (!File.Exists(to))
        {
            return false;
        }
            
        return true;
    }

    public void DeployFile(string from, string to)
    {
        File.Copy(from, to);
    }
}