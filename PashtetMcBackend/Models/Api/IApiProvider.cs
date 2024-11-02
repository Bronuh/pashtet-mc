using Common.Api;
using Microsoft.AspNetCore.Mvc;

namespace BronuhMcBackend.Models.Api;

public interface IApiProvider
{
    LocalFile GetJavaFile();
    LocalFile GetMinecraftFile();

    IEnumerable<LocalFile> GetRequiredModsList();
    IEnumerable<LocalFile> GetOptionalModsList();
    
    LocalFile GetRequiredModFile(string modName);
    LocalFile GetOptionalModFile(string modName);
    
    LocalFile GetServersFile();

    void Initialize();
}