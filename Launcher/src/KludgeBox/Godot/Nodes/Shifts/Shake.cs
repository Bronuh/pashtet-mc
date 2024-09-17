#region

using KludgeBox.Core;

#endregion

namespace KludgeBox.Godot.Nodes.Shifts;

public class Shake(real strength, double time, bool deceising = true) : IShiftProvider
{
    public double Time { get; private set; } = time;
    public double InitialTime { get; private set; } = time;

    public real Strength => InitialStrength * (real)(Time / InitialTime);
    public real InitialStrength { get; private set; } = strength;

    public Vector2 Shift => Rand.InsideUnitCircle * (deceising ? Strength : InitialStrength);
    public bool IsAlive => Time > Mathf.Epsilon;
		
    public void Update(double delta)
    {
        Time = Mathf.Max(0, Time - delta);
    }
}