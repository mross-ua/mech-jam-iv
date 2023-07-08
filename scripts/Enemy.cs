using Godot;
using System;
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

}
