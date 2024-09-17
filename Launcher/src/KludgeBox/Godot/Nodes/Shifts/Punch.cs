namespace KludgeBox.Godot.Nodes.Shifts;

public class Punch(Vector2 dir, real strength, real movementSpeed = 3000) : IShiftProvider
{
    public Vector2 Direction { get; private set; } = dir.Normalized();
    public real Strength { get; private set; } = strength;
    public real InitialStrength { get; private set; } = strength;

    public Vector2 Shift => Direction * Strength;
    public bool IsAlive => Strength > Mathf.Epsilon;
    public void Update(double delta)
    {
        Strength = Mathf.Max(0, Strength - movementSpeed * (real)delta);
    }
}