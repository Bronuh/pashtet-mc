using Common.Api;
using IO;
using Newtonsoft.Json;

namespace Api;

public class DefaultApiProvider : IApiProvider
{
    public string Host { get; set; } = "https://minecraft.bronuh.ru";
    
    public string RequiredModsRoute { get; set; } = "api/v1/mods/required/list";
    public string OptionalModsRoute { get; set; } = "api/v1/mods/optional/list";
    public string JavaRoute { get; set; } = "api/v1/core/jre";
    public string MinecraftRoute { get; set; } = "api/v1/core/minecraft";
    
    public string RequiredModsUrl => $"{Host}/{RequiredModsRoute}";
    public string OptionalModsUrl => $"{Host}/{OptionalModsRoute}";
    public string JavaUrl => $"{Host}/{JavaRoute}";
    public string MinecraftUrl => $"{Host}/{MinecraftRoute}";

    public string GetJavaUrl()
    {
        return JavaUrl;
    }

    public string GetMinecraftUrl()
    {
        return MinecraftUrl;
    }

    public RemoteFilesList GetRequiredModsList()
    {
        var result = HttpHelper.Get(RequiredModsUrl).Body;
        var mods = JsonConvert.DeserializeObject<RemoteFilesList>(result);
        return mods;
    }

    public RemoteFilesList GetOptionalModsList()
    {
        var result = HttpHelper.Get(OptionalModsUrl).Body;
        var mods = JsonConvert.DeserializeObject<RemoteFilesList>(result);
        return mods;
    }
}