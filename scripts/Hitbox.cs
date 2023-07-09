using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Hitbox : Area2D
{

	[Signal]
	public delegate void HitEventHandler(int damage, Vector2 direction);

	[Export]
	public int Damage { get; set; } = 10;
	[Export]
	public bool IsWeakSpot { get; set; } = false;
	[Export]
	public int CriticalDamageMultiplier { get; set; } = 2;

	private Dictionary<Rid, Player> collidingBodies = new ();

	public override void _Ready()
	{
		BodyEntered += (body) =>
		{
			if (body is Player player)
			{
				collidingBodies.Add(player.GetRid(), player);

				player.HurtAsync(Damage, Vector2.Zero);
			}
		};
		BodyExited += (body) =>
		{
			if (body is Player player)
			{
				collidingBodies.Remove(player.GetRid());
			}
		};
	}

	public override void _PhysicsProcess(double delta)
	{
		if (collidingBodies.Any())
		{
			foreach (KeyValuePair<Rid, Player> kvp in collidingBodies)
			{
				kvp.Value.HurtAsync(Damage, Vector2.Zero);
			}
		}
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
