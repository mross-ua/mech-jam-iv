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

	protected override Vector2 GetMovementDirection(double delta) => base.GetMovementDirection(delta);

    protected override void ProcessAttack(double delta)
    {
        throw new NotImplementedException();
    }

    protected override void AnimateMovement(Vector2 direction, double delta)
    {
		if (Mathf.IsZeroApprox(direction.X))
		{
			animatedSprite2D.Play("idle");
		}
		else
		{
			animatedSprite2D.Play("run");
		}
    }

    protected override void AnimateDeath()
    {
		animatedSprite2D.Play("death");
    }

}
