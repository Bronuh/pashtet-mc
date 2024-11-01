namespace Common.FileSystem.Deploy;

/// <summary>
/// Implementation of the <see cref="IFileDeployer"/> interface that combines multiple deployers to select the appropriate one.
/// </summary>
public class CombinedDeployer : IFileDeployer
{
    private readonly IFileDeployer[] _deployers;

    /// <summary>
    /// Creates a new instance of the <see cref="CombinedDeployer"/> class that accepts a set of deployers.
    /// </summary>
    /// <param name="deployers">An array of objects implementing the <see cref="IFileDeployer"/> interface.</param>
    public CombinedDeployer(params IFileDeployer[] deployers)
    {
        _deployers = deployers;
    }
    
    /// <summary>
    /// Determines whether deployment from the given source to the specified path can be performed using one of the available deployers.
    /// </summary>
    /// <param name="from">The path to the source file.</param>
    /// <param name="to">The path to the target file.</param>
    /// <returns>Returns <see langword="true"/> if there is at least one deployer that can perform the deployment; otherwise, returns <see langword="false"/>.</returns>
    public bool IsDeployable(string from, string to)
    {
        return _deployers.FirstOrDefault(deployer => deployer.IsDeployable(from, to)) is not null;
    }

    /// <summary>
    /// Deploys a file from the given source to the specified path using the first suitable deployer.
    /// </summary>
    /// <param name="from">The path to the source file.</param>
    /// <param name="to">The path to the target file.</param>
    /// <exception cref="InvalidOperationException">Thrown if there is no deployer that can perform the deployment.</exception>
    public void DeployFile(string from, string to)
    {
        var deployer = _deployers.First(deployer => deployer.IsDeployable(from, to));
        deployer.DeployFile(from, to);
    }
}
