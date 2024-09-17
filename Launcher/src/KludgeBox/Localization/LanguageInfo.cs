#region

using System.IO;
using Newtonsoft.Json.Linq;
using FileAccess = Godot.FileAccess;

#endregion

namespace KludgeBox.Localization;

public class LanguageInfo
{
    public string LanguageName { get; private set; }
    public string LanguageCode { get; private set; }
    
    public LanguageInfo(string json)
    {
        var node = JObject.Parse(json);
        LanguageName = node[nameof(LanguageName)]!.ToString();
    }
    
    public static LanguageInfo FromJsonFile(string path)
    {
        var fileAccess = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if(fileAccess.GetError() is not Error.Ok) throw new IOException(
            $"Unable to open file {path}: {fileAccess.GetError()}");
        var json = fileAccess.GetAsText();
        var info = new LanguageInfo(json);
        info.LanguageCode = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(path));
        
        return info;
    }
}