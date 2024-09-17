namespace KludgeBox.Localization;

public static class Translator
{
    public static string Translate(string key, params object[] args)
    {
        args ??= [];
        var text = LanguageDatabase.CurrentLanguage.Data.Get(key);
        string result = text;
        try
        {
            result = string.Format(text, args);
        }
        catch(Exception e)
        {
            Log.Error(e);
        }

        return result;
    }

    public static string RawTranslate(string key)
    {
        return LanguageDatabase.CurrentLanguage.Data.Get(key);
    }
}