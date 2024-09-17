#region

using KludgeBox.Godot.Extensions;
using KludgeBox.Godot.Nodes.Shifts;

#endregion

namespace KludgeBox.Godot.Nodes;

[GlobalClass]
public partial class SmoothCamera2D : Camera2D
{
	[Export] public Node2D TargetNode;
	[Export] public real SmoothingBase = 0.1f;
	[Export] public real SmoothingPower = 2f; // The power to which the SmoothingBase value will be raised
	
	public Vector2 TargetPosition = Vector2.Zero; // Position where camera wants to be
	public Vector2 ActualPosition = Vector2.Zero; // Position where camera is currently in
	public Vector2 PositionShift = Vector2.Zero; // Additional shift to ActualPosition. WILL be smoothed. NOT usable for punching and shaking.
	public Vector2 HardPositionShift = Vector2.Zero; // Additional shift to ActualPosition. Will NOT be smoothed. USABLE for punching and shaking.
	// Note that PlayerCamera.Position is always being calculated to represent additional PositionShift
	// from ActualPosition (e.g. shake or punch).
	// ActualPosition represents main movement between current and desired position. It mostly used for custom smoothing.

	public List<IShiftProvider> Shifts = new();

	public Vector2 AdditionalShift
	{
		get
		{
			var shift = Vec();
			foreach (IShiftProvider punch in Shifts)
			{
				shift += punch.Shift;
			}

			return shift;
		}
	}
	
	
	public override void _Ready()
	{
		ActualPosition = Position;
		TargetPosition = Position;
	}

	private const real highFpsDelta = 1f / 240;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		MoveCamera((float)delta);
		UpdateShifts((float)delta);
	}
	
	private void MoveCamera(float delta)
	{
		if (!TargetNode.IsValid()) return;
        
		TargetPosition = TargetNode.GlobalPosition;
		var availableMovement = (TargetPosition + PositionShift) - ActualPosition;
		var actualMovement = availableMovement * Mathf.Pow(SmoothingBase, SmoothingPower) * (delta / highFpsDelta);
		
		ActualPosition += actualMovement;
		Position = ActualPosition + HardPositionShift + AdditionalShift;
	}

	private void UpdateShifts(double delta)
	{
		foreach (var punch in Shifts)
		{
			punch.Update(delta);
		}

		Shifts.RemoveAll(s => !s.IsAlive);
	}
	
	public Punch Punch(Vector2 dir, real strength, real movementSpeed = 3000)
	{
		var punch = new Punch(dir, strength, movementSpeed);
		Shifts.Add(punch);
		return punch;
	}

	public Shake Shake(real strength, double time, bool deceising = true)
	{
		var shake = new Shake(strength, time, deceising);
		Shifts.Add(shake);
		return shake;
	}

	public ManualShake ShakeManually()
	{
		var shake = new ManualShake();
		Shifts.Add(shake);
		return shake;
	}
}