namespace KludgeBox.Localization;

public class LocalizedText
{
    public string Key { get; set; }
    public object[] Args { get; set; }

    public LocalizedText(string key, params object[] args)
    {
        Key = key;
        Args = args;
    }

    public LocalizedText(string key)
    {
        Key = key;
        Args = Array.Empty<object>();
    }

    public override string ToString()
    {
        return Translator.Translate(Key, Args);
    }
    
    public static implicit operator string(LocalizedText text) => text.ToString();
}
    