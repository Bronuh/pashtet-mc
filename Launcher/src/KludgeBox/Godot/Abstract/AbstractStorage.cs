#region

using KludgeBox.Core;

#endregion

namespace KludgeBox.Godot.Abstract;

public abstract partial class AbstractStorage : Node
{
    public override void _Ready()
    {
        NotNullChecker.CheckProperties(this);
    }
}