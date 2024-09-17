namespace KludgeBox.Godot;

public static partial class Utils 
{
    public static SceneTree SceneTree => (SceneTree)Engine.GetMainLoop();
}