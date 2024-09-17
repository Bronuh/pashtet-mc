namespace KludgeBox.Godot.Extensions;

public static class ControlExtensions
{
    public static void SetStyleBox(this Control control, StyleBox style, string themeName = "panel")
    {
        control.RemoveThemeStyleboxOverride(themeName);
        control.AddThemeStyleboxOverride(themeName, style);
    }

    public static StyleBox GetStyleBox(this Control control, string themeName = "panel")
    {
        return control.GetThemeStylebox(themeName);
    }
}