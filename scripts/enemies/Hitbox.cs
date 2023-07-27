using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Hitbox : Area2D
{

	[Signal]
	public delegate void HitEventHandler(int damage, bool isWeakSpot, Vector2 globalPos, Vector2 normal);
	[Signal]
	public delegate void CollidingEventHandler(Node2D body);

	[Export]
	public int Damage { get; set; } = 10;

	[Export]
	public bool IsWeakSpot { get; set; } = false;

	public override void _Ready()
	{
		BodyEntered += (body) =>
		{
			if (body is Player player)
			{
				EmitSignal(SignalName.Colliding, player);
			}
		};
	}

	public override void _PhysicsProcess(double delta)
	{
		foreach (Node2D body in GetOverlappingBodies())
		{
			if (body is Player player)
			{
				EmitSignal(SignalName.Colliding, player);
			}
		}
	}

	#region ICollidable

	public void Hurt(int damage, Vector2 globalPos, Vector2 normal)
	{
		EmitSignal(SignalName.Hit, damage, IsWeakSpot, globalPos, normal);
	}

	#endregion

}
