#region

using System.Threading.Tasks;
using Common.Api;
using IO;
using Newtonsoft.Json;

#endregion

namespace WebApi;

public class DefaultApiProvider : IApiProvider
{
    public string Host { get; set; } = "https://minecraft.bronuh.ru";
    
    public string RequiredModsRoute { get; set; } = "api/v1/mods/required/list";
    public string OptionalModsRoute { get; set; } = "api/v1/mods/optional/list";
    public string JavaRoute { get; set; } = "api/v1/core/jre";
    public string MinecraftRoute { get; set; } = "api/v1/core/minecraft";
    public string ServersRoute { get; set; } = "api/v1/data/servers";
    public string PatchInfoRoute { get; set; } = "api/v1/patch/info";
    
    public string RequiredModsUrl => $"{Host}/{RequiredModsRoute}";
    public string OptionalModsUrl => $"{Host}/{OptionalModsRoute}";
    public string OptionalModsInfoUrl => $"{Host}/{OptionalModsRoute}/info";
    public string JavaUrl => $"{Host}/{JavaRoute}";
    public string MinecraftUrl => $"{Host}/{MinecraftRoute}";
    public string ServersUrl => $"{Host}/{ServersRoute}";
    public string PatchInfoUrl => $"{Host}/{PatchInfoRoute}";

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
        try
        {
            var infoUrl = GetMinecraftUrl() + "/info";
            var result = await HttpHelper.GetAsync(infoUrl);
            var info = JsonConvert.DeserializeObject<RemoteFile>(result.Body);
        
            return info;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return null;
        }
    }

    public async Task<RemoteFile> GetJavaInfoAsync()
    {
        try
        {
            var infoUrl = GetJavaUrl() + "/info";
            var result = await HttpHelper.GetAsync(infoUrl);
            var info = JsonConvert.DeserializeObject<RemoteFile>(result.Body);
        
            return info;

        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return null;
        }
    }

    public async Task<RemoteFile> GetPatchInfoAsync()
    {
        RemoteFile info = null;
        try
        {
            var infoUrl = PatchInfoUrl;
            var result = await HttpHelper.GetAsync(infoUrl);
            info = JsonConvert.DeserializeObject<RemoteFile>(result.Body);
        }
        catch
        {
            Log.Warning($"Удаленный патч не найден");
        }
        
        return info;
    }

    public async Task<RemoteFilesList> GetRequiredModsListAsync()
    {
        var result = await HttpHelper.GetAsync(RequiredModsUrl, ignoreException: true);
        if (result is null)
            return null;
        
        var mods = JsonConvert.DeserializeObject<RemoteFilesList>(result.Body);
        return mods;
    }

    public async Task<RemoteFilesList> GetOptionalModsListAsync()
    {
        var result = await HttpHelper.GetAsync(OptionalModsUrl, ignoreException: true);
        if (result is null)
            return null;
        
        var mods = JsonConvert.DeserializeObject<RemoteFilesList>(result.Body);
        return mods;
    }

    public async Task<List<ModInfo>> GetOptionalModsInfoAsync()
    {
        var result = await HttpHelper.GetAsync(OptionalModsInfoUrl, ignoreException: true);
        if (result is null)
            return null;
        
        var mods = JsonConvert.DeserializeObject<List<ModInfo>>(result.Body);
        return mods;
    }
}