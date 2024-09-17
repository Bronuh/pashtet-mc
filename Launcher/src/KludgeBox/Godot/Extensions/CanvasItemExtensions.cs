namespace KludgeBox.Godot.Extensions;

public static class CanvasItemExtensions
{
    public static void SetShaderParameter(this CanvasItem item, string name, Variant value)
    {
        var shader = item.Material as ShaderMaterial;
        if (shader is not null)
        {
            shader.SetShaderParameter(name, value);
        }
        else
        {
            Log.Warning($"Attempted to set shader parameter '{name}' on non-shader material '{item.Material}'");
        }
    }
    
    public static Variant GetShaderParameter(this CanvasItem item, string name)
    {
        var shader = item.Material as ShaderMaterial;
        if (shader is not null)
        {
            return shader.GetShaderParameter(name);
        }
        else
        {
            Log.Warning($"Attempted to get shader parameter '{name}' on non-shader material '{item.Material}'");
            return default;
        }
    }
}