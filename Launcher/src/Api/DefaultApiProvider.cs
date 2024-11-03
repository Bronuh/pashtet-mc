#region

using System.Threading.Tasks;
using Common.Api;
using IO;
using Newtonsoft.Json;

#endregion

namespace Api;

public class DefaultApiProvider : IApiProvider
{
    public string Host { get; set; } = "https://minecraft.bronuh.ru";
    
    public string RequiredModsRoute { get; set; } = "api/v1/mods/required/list";
    public string OptionalModsRoute { get; set; } = "api/v1/mods/optional/list";
    public string JavaRoute { get; set; } = "api/v1/core/jre";
    public string MinecraftRoute { get; set; } = "api/v1/core/minecraft";
    public string ServersRoute { get; set; } = "api/v1/data/servers";
    
    public string RequiredModsUrl => $"{Host}/{RequiredModsRoute}";
    public string OptionalModsUrl => $"{Host}/{OptionalModsRoute}";
    public string JavaUrl => $"{Host}/{JavaRoute}";
    public string MinecraftUrl => $"{Host}/{MinecraftRoute}";
    public string ServersUrl => $"{Host}/{ServersRoute}";

    public string GetJavaUrl()
    {
        return JavaUrl;
    }


    public string GetMinecraftUrl()
    {
        return MinecraftUrl;
    }

    public string GetServersUrl()
    {
        return ServersUrl;
    }

    public async Task<RemoteFile> GetMinecraftInfoAsync()
    {
        var infoUrl = GetMinecraftUrl() + "/info";
        var result = await HttpHelper.GetAsync(infoUrl);
        var info = JsonConvert.DeserializeObject<RemoteFile>(result.Body);
        
        return info;
    }

    public async Task<RemoteFile> GetJavaInfoAsync()
    {
        var infoUrl = GetJavaUrl() + "/info";
        var result = await HttpHelper.GetAsync(infoUrl);
        var info = JsonConvert.DeserializeObject<RemoteFile>(result.Body);
        
        return info;
    }

    public async Task<RemoteFilesList> GetRequiredModsListAsync()
    {
        var result = await HttpHelper.GetAsync(RequiredModsUrl);
        var mods = JsonConvert.DeserializeObject<RemoteFilesList>(result.Body);
        return mods;
    }

    public async Task<RemoteFilesList> GetOptionalModsListAsync()
    {
        var result = await HttpHelper.GetAsync(OptionalModsUrl);
        var mods = JsonConvert.DeserializeObject<RemoteFilesList>(result.Body);
        return mods;
    }
}