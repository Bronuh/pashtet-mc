namespace KludgeBox.Localization;

public class LanguageBundle
{
    public LanguageData Data { get; private set; }
    public LanguageInfo Info { get; private set; }
    
    public LanguageBundle(LanguageData data, LanguageInfo info)
    {
        Data = data;
        Info = info;
    }
    
    public static LanguageBundle FromDirectory(string path)
    {
        var data = LanguageData.FromJsonFile($"{path}/Keyed.json5");
        var info = LanguageInfo.FromJsonFile($"{path}/About.json5");
        return new LanguageBundle(data, info);
    }
}