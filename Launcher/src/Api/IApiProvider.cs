using Common.Api;

namespace Api;

public interface IApiProvider
{
    public string GetJavaUrl();
    public string GetMinecraftUrl();
    public RemoteFilesList GetRequiredModsList();
    public RemoteFilesList GetOptionalModsList();
}