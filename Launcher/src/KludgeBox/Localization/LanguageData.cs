#region

using System.Collections.ObjectModel;
using System.IO;
using KludgeBox.Collections;
using Newtonsoft.Json.Linq;
using FileAccess = Godot.FileAccess;

#endregion

namespace KludgeBox.Localization;

public class LanguageData
{
    private Dictionary<string, string> _translations = new();
    public ReadOnlyDictionary<string, string> Translations => _translations.AsReadOnly();
    
    public string Get(string key) => _translations.GetValueOrDefault(key, key) ?? "NULL";


    public void AddTranslation(string key, string value)
    {
        _translations[key] = value;
    }

    public void AddTranslations(IDictionary<string, string> translations)
    {
        foreach (var (key, value) in translations)
        {
            _translations[key] = value;
        }
    }
    
    public static LanguageData FromJson(string json)
    {
        var node = JObject.Parse(json);
        var data = new LanguageData();
        foreach (var (key, value) in node)
        {
            data._translations[key] = value.ToString();
        }
        return data;
    }
    
    public static LanguageData FromJsonFile(string path)
    {
        var fileAccess = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if(fileAccess.GetError() is not Error.Ok) throw new IOException(
            $"Unable to open file {path}: {fileAccess.GetError()}");
        var json = fileAccess.GetAsText();
        return FromJson(json);
    }
}