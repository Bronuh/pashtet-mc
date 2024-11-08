#region

using Common.Api;

#endregion

namespace PashtetMcBackend.Models.Api;

public interface IApiProvider
{
    LocalFile GetJavaFile();
    LocalFile GetMinecraftFile();
    LocalFile GetVersionFile();
    LocalFile GetPatchFile();

    IEnumerable<LocalFile> GetRequiredModsList();
    IEnumerable<LocalFile> GetOptionalModsList();
    IEnumerable<ModInfo> GetOptionalModsInfoList();
    
    LocalFile GetRequiredModFile(string modName);
    LocalFile GetOptionalModFile(string modName);
    
    LocalFile GetServersFile();

    void Initialize();
}