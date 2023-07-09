using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MechJamIV;

public partial class Enemy : CharacterBase
{

	#region Node references

	private AnimatedSprite2D animatedSprite2D;

	#endregion

    public override void _Ready()
    {
		base._Ready();

        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		foreach (Node2D node in GetNode<Node2D>("Hitboxes").GetChildren())
		{
			if (node is Hitbox hitbox)
			{
				hitbox.Hit += (damage, normal) => HurtAsync(damage, normal);
			}
		}
    }

	protected override Vector2 GetMovementDirection(double delta)
	{
		Vector2 direction = Vector2.Zero;

		if (Mathf.IsZeroApprox(direction.X))
		{
			animatedSprite2D.Play("idle");
		}
		else
		{
			animatedSprite2D.Play("run");
		}

		return direction;
	}

	public override async void HurtAsync(int damage, Vector2 normal)
	{
		if (Health <= 0)
		{
			return;
		}

		base.HurtAsync(damage, normal);

		if (Health <= 0)
		{
			animatedSprite2D.Play("death");

			await ToSignal(GetTree().CreateTimer(5.0f), SceneTreeTimer.SignalName.Timeout);

			QueueFree();
		}
	}

}
