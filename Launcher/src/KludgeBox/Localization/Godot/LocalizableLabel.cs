namespace KludgeBox.Localization.Godot;

[GlobalClass]
public partial class LocalizableLabel : Label, ILocalizable
{
    [Export] public string Key { get; set; }
    public object[] Args { get; set; }
    
    public LocalizableLabel(){}
    
    public LocalizableLabel(string key, params object[] args)
    {
        Key = key;
        Args = args;
    }

    /// <inheritdoc />
    public override void _Ready()
    {
        UpdateLanguage();
    }

    /// <inheritdoc />
    public void UpdateLanguage()
    {
        Text = Translator.Translate(Key, Args);
    }
}