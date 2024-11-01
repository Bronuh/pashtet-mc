#region

using Common.Platform;

#endregion

namespace Common.FileSystem.Deploy;

public class UnixHardLinkDeployer : IFileDeployer
{
    public IHardLinkDeploymentChecker Checker { get; set; } = new UnixHardLinkDeploymentChecker();
    
    public bool IsDeployable(string from, string to)
    {
        return Checker.IsHardLinkDeploymentAvailable(from, to);
    }

    public void DeployFile(string from, string to)
    {
        Runner.Run($"ln {from} {to}");
    }
}