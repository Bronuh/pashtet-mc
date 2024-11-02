using System.Threading.Tasks;
using Common.Api;

namespace Api;

public interface IApiProvider
{
    public string GetJavaUrl();
    public string GetMinecraftUrl();
    public string GetServersUrl();
    public Task<RemoteFilesList> GetRequiredModsListAsync();
    public Task<RemoteFilesList> GetOptionalModsListAsync();
}