#region

using Common.FileSystem.Deploy;
using Common.IO.Checksum;
using Common.Logging;
using Common.Password;
using Common.Platform;
using JetBrains.Annotations;
using SimpleInjector;

// ReSharper disable RedundantTypeArgumentsOfMethod

#endregion

namespace Common;


/// <summary>
/// This class is used exclusively for synchronizing strategies between the client and server sides. 
/// The references stored here should ONLY be used during application initialization.
/// </summary>
[PublicAPI]
public static class DefaultServices
{
    static DefaultServices()
    {
        var platform = Utils.GetPlatform();
            
        if (platform is PlatformType.Windows)
        {
            HardLinkChecker = new WindowsHardLinkDeploymentChecker();
            FileDeployer = new WindowsHardLinkDeployer();
        }
        else if (platform is PlatformType.Unix)
        {
            HardLinkChecker = new UnixHardLinkDeploymentChecker();
            FileDeployer = new UnixHardLinkDeployer();
        }
        else
        {
            HardLinkChecker = new NoHardlinkDeploymentChecker();
            FileDeployer = new CopyDeployer();
        }
        
        // Combine main deployer and fallback deployer
        FileDeployer = new CombinedDeployer(
            FileDeployer, FallbackFileDeployer);
    }
    
    
    public static IPasswordHasher PasswordHasher { get; set; } = new Rfc2898PasswordHasher();
    public static IHardLinkDeploymentChecker HardLinkChecker { get; set; }
    public static IFileDeployer FileDeployer { get; set; }
    public static IFileDeployer FallbackFileDeployer { get; set; } = new CopyDeployer();
    public static ILogger Logger { get; set; } = new ConsoleLogger();
    public static IChecksumProvider ChecksumProvider { get; set; } = new Xx3ChecksumProvider();
    

    /// <summary>
    /// Converts the default services into a dependency injection container.
    /// </summary>
    /// <returns>A <see cref="Container"/> object containing the registered services.</returns>
    public static Container AsDiContainer()
    {
        var container = new Container();
        container.Options.DefaultLifestyle = Lifestyle.Singleton;
        
        container.RegisterInstance<IPasswordHasher>(PasswordHasher);
        container.RegisterInstance<IHardLinkDeploymentChecker>(HardLinkChecker);
        container.RegisterInstance<IFileDeployer>(new CombinedDeployer(
            FileDeployer, FallbackFileDeployer));
        container.RegisterInstance<ILogger>(Logger);
        container.RegisterInstance<IChecksumProvider>(ChecksumProvider);
        
        return container;
    }
}