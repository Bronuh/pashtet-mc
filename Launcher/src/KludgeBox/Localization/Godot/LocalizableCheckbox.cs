#region

using KludgeBox.Collections;

#endregion

namespace KludgeBox.Localization.Godot;

[GlobalClass]
public partial class LocalizableCheckbox : CheckBox, ILocalizable
{
    //public static RandomPicker<string> DefaultPressSounds => Sfx.UiClick;
    [Export] public string Key { get; set; }
    public RandomPicker<string> PressSounds { get; set; }
    public object[] Args { get; set; }
    
    public LocalizableCheckbox(){}
    
    public LocalizableCheckbox(string key, params object[] args)
    {
        Key = key;
        Args = args;
    }

    /// <inheritdoc />
    public override void _Ready()
    {
        //PressSounds ??= DefaultPressSounds;
        //Pressed += () => Audio2D.PlayUiSound(PressSounds);
        UpdateLanguage();
    }

    /// <inheritdoc />
    public void UpdateLanguage()
    {
        Text = Translator.Translate(Key, Args);
    }
}