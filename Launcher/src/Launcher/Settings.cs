using KludgeBox.Core;
using KludgeBox.Localization;
using Newtonsoft.Json;

namespace Launcher;


public sealed class Settings
{
    public double MaxRam;
    public string PlayerName;
    public string Password;

    public Settings()
    {
        MaxRam = Main.GetInstalledRamAmount() / 2;
        PlayerName = $"{(new string[] {"Biba", "Boba", "Afafaf", "Mingebag", "Aboba", "Oof"}).GetRandom()}{Rand.Int}";
        Password = "";
    }
}

public static class SettingsUtils
{
    public const string SettingsPath = "user://settings.json";

    public static Settings Clone(Settings source)
    {
        return JsonConvert.DeserializeObject<Settings>(JsonConvert.SerializeObject(source));
    }
    
    public static Settings LoadSettings()
    {
        var file = FileAccess.Open(SettingsPath, FileAccess.ModeFlags.Read);
        try
        {
            var text = file.GetAsText();
            return JsonConvert.DeserializeObject<Settings>(text) ?? new Settings();
        }
        catch
        {
            
        }
        
        file?.Close();
        
        var settings = new Settings();
        return settings;
    }
    
    public static void SaveSettings(Settings settings)
    {
        var file = FileAccess.Open(SettingsPath, FileAccess.ModeFlags.Write);
        try
        {
            var text = JsonConvert.SerializeObject(settings, Formatting.Indented);
            file.StoreString(text);
        }
        catch
        {
            
        }
        file?.Close();
    }
}