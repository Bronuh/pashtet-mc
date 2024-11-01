namespace Common.FileSystem.Deploy;

public interface IHardLinkDeploymentChecker
{
    /// <summary>
    /// Determines if this directory and it's subdirectories supports hard links creation.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    bool IsHardLinkDeploymentAvailable(string path);
    
    bool IsHardLinkDeploymentAvailable(string from, string to);
}