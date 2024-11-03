#region

using System.Threading.Tasks;
using Common.Api;

#endregion

namespace Api;

public interface IApiProvider
{
    string GetJavaUrl();
    string GetMinecraftUrl();
    string GetServersUrl();
    Task<RemoteFile> GetMinecraftInfoAsync();
    Task<RemoteFile> GetJavaInfoAsync();
    Task<RemoteFilesList> GetRequiredModsListAsync();
    Task<RemoteFilesList> GetOptionalModsListAsync();
}