#region

using System.Globalization;

#endregion

namespace KludgeBox.Localization;

public static class LanguageDatabase
{
    private static Dictionary<string, LanguageBundle> _bundles = new();

    public static LanguageBundle CurrentLanguage;

    public static string DefaultLanguage => "en";
    public static string FallbackLanguage => "ru";
    
    public static string SystemLanguage => CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
    
    public static IEnumerable<string> AvailableLanguages => _bundles.Keys;
    
    public static void SelectLanguage(string language)
    {
        CurrentLanguage = _bundles.TryGetValue(language, out var bundle)
            ? bundle
            : _bundles.TryGetValue(DefaultLanguage, out var defaultBundle)
            ? defaultBundle:
            _bundles[FallbackLanguage];
    }
    
    public static void LoadBundle(string path)
    {
        var bundle = LanguageBundle.FromDirectory(path);
        _bundles[bundle.Info.LanguageCode] = bundle;
    }
}