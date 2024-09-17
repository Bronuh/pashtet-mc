using System.Linq;

namespace KludgeBox.Localization.Types;

public sealed class LocalizableString: ILocalizable
{
    public string LocalizationKey { get; set; }
    public object[] LocalizationArgs { get; set; }
    
    public string CachedLocalizedString { get; private set; }
    
    private string _activeLocalizationKey;
    private object[] _activeLocalizationArgs;
    
    private bool IsDirty => _activeLocalizationKey != LocalizationKey || _activeLocalizationArgs != LocalizationArgs;
    
    public override string ToString()
    {
        if (IsDirty)
        {
            UpdateLanguage();
        }
        
        return CachedLocalizedString;
    }
    
    public LocalizableString(string localizationKey, params object[] localizationArgs)
    {
        LocalizationKey = localizationKey;
        LocalizationArgs = new object[localizationArgs?.Length ?? 0];
        localizationArgs?.CopyTo(LocalizationArgs, 0);
    }

    /// <inheritdoc />
    public void UpdateLanguage()
    {
        CachedLocalizedString = Translator.Translate(LocalizationKey, CachedLocalizedString);
    }
    
    public static implicit operator LocalizableString(string localizationKey) => new(localizationKey);
    public static implicit operator string(LocalizableString localizableString) => localizableString.ToString();
}