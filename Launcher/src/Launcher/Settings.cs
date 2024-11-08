﻿#region

using KludgeBox.Core;
using Launcher.Nodes;
using Newtonsoft.Json;

#endregion

namespace Launcher;


public sealed class Settings
{
    public string this[string key]
    {
        get => GetCustom(key);
        set => SetCustom(key, value);
    }
    
    public double MaxRam;
    public string PlayerName;
    public string Password;

    [JsonProperty]
    private Dictionary<string, string> _customSettings;

    public Settings()
    {
        MaxRam = Main.GetInstalledRamAmount() / 2;
        PlayerName = $"{(new string[] {"Biba", "Boba", "Afafaf", "Mingebag", "Aboba", "Oof"}).GetRandom()}{Rand.Int}";
        Password = "";
        _customSettings = new Dictionary<string, string>();
    }

    public string GetCustom(string key)
    {
        if (_customSettings.TryGetValue(key, out string value))
        {
            return value;
        }

        var defaultValue = "";
        SetCustom(key, defaultValue);
        
        return defaultValue;
    }

    public void SetCustom(string key, string value)
    {
        _customSettings[key] = value;
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