#region

using System.Reflection;
using KludgeBox.Core;

#endregion

namespace KludgeBox.Godot.Abstract;

public partial class ScenesStorage : AbstractStorage
{
    private Dictionary<string, PackedScene> _scenes = new();
    /// <inheritdoc />
    public override void _Ready()
    {
        base._Ready();
        RegisterScenes(this);
    }
    public bool TryGetScene(string name, out PackedScene scene)
    {
        return _scenes.TryGetValue(name, out scene);
    }
    
    private void RegisterScenes(object obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        Type type = obj.GetType();
        foreach (PropertyInfo property in type.GetProperties())
        {
            if (!property.PropertyType.IsAssignableTo(typeof(PackedScene))) continue;
            if (!Attribute.IsDefined(property, typeof(ExportAttribute))) continue;
            if (!Attribute.IsDefined(property, typeof(NotNullAttribute))) continue;

            _scenes[property.Name] = property.GetValue(this) as PackedScene;
        }
    }
}