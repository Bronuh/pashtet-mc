#region

using KludgeBox.Collections;

#endregion

namespace KludgeBox.Localization.Godot;

[GlobalClass]
public partial class LocalizableButton : Button, ILocalizable
{
    //public static RandomPicker<string> DefaultPressSounds => Sfx.UiClick;
    [Export] public string Key { get; set; }
    public RandomPicker<string> PressSounds { get; set; }
    public object[] Args { get; set; }
    
    public LocalizableButton(){}
    
    public LocalizableButton(string key, params object[] args)
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