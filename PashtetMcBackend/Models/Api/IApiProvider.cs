#region

using Common.Api;

#endregion

namespace PashtetMcBackend.Models.Api;

public interface IApiProvider
{
    LocalFile GetJavaFile();
    LocalFile GetMinecraftFile();
    LocalFile GetVersionFile();

    IEnumerable<LocalFile> GetRequiredModsList();
    IEnumerable<LocalFile> GetOptionalModsList();
    
    LocalFile GetRequiredModFile(string modName);
    LocalFile GetOptionalModFile(string modName);
    
    LocalFile GetServersFile();

    void Initialize();
}