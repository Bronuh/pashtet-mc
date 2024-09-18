namespace Launcher;

public static class Urls
{
    public const string BaseUrl = "https://minecraft.bronuh.ru/api/";
    public const string JreUrl = BaseUrl + "core/jre/";
    public const string MinecraftUrl = BaseUrl + "core/minecraft/";
    
    public const string ModsUrl = BaseUrl + "directory/mods/list/";
    public const string FileDownloadUrl = BaseUrl + "directory/download/";
    public const string FileSnapshotDownloadUrl = BaseUrl + "directory/download/snapshots/current/";
    public const string ModsDownloadBaseUrl = FileSnapshotDownloadUrl + "mods/";

    public static string GetModDownloadUrl(string modName)
    {
        return $"{ModsDownloadBaseUrl}{modName}";
    }

    public static string GetFileDownloadUrl(string path)
    {
        return $"{FileDownloadUrl}{path}";
    }
}