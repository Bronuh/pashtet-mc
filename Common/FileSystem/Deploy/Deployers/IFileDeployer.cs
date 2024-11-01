namespace Common.FileSystem.Deploy;

public interface IFileDeployer
{
    /// <summary>
    /// Determines whether this deployer is capable of deploying the file to the specified destination.
    /// </summary>
    /// <param name="from">The path to the source file that needs to be deployed.</param>
    /// <param name="to">The path to the target destination where the file should be deployed.</param>
    /// <returns>True if the file can be deployed to the specified destination; otherwise, false.</returns>
    bool IsDeployable(string from, string to);
    
    /// <summary>
    /// Deploys the file to the specified destination.
    /// </summary>
    /// <remarks>
    /// The source file must remain intact and should not be deleted after deployment.
    /// </remarks>
    /// <param name="from">The path to the source file that needs to be deployed.</param>
    /// <param name="to">The path to the target destination where the file should be deployed.</param>
    void DeployFile(string from, string to);
    
    /// <summary>
    /// Cancels the deployment of the file by removing the deployed file.
    /// </summary>
    /// <param name="path">The path to the deployed file that should be removed.</param>
    void UndeployFile(string path) => File.Delete(path);
}