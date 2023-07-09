using Godot;
using System;

public partial class Hitbox : Area2D
{

	[Signal]
	public delegate void HitEventHandler(int damage, Vector2 direction);

	[Export]
	public bool IsWeakSpot { get; set; } = false;
	[Export]
	public int CriticalDamageMultiplier { get; set; } = 2;

	public override void _Ready()
	{

	}

	public override void _Process(double delta)
	{

	}

	public void Hurt(int damage, Vector2 direction)
	{
		if (IsWeakSpot)
		{
			EmitSignal(SignalName.Hit, damage * CriticalDamageMultiplier, direction);
		}
		else
		{
			EmitSignal(SignalName.Hit, damage, direction);
		}
	}

}
