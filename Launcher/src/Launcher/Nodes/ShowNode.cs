namespace Launcher.Nodes;


/// <summary>
/// This node just makes its parent visible on start.
/// Use this if you want to hide something in editor that must be visible in runtime.
/// </summary>
[GlobalClass]
public partial class ShowNode : Node
{
    public override void _Ready()
    {
        var parent = GetParent();
        if (parent is CanvasItem canvasItem)
        {
            canvasItem.Visible = true;
        }
        QueueFree();
    }
}