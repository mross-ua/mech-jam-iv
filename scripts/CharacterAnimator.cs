using Godot;
using System;

public partial class CharacterAnimator : AnimatedSprite2D
{

	public void AnimateMovement(Vector2 direction, double delta)
	{
		if (Mathf.IsZeroApprox(direction.X))
		{
			Play("idle");
		}
		else
		{
			Play("run");
		}
	}

	public void AnimateDeath()
	{
		Play("death");
	}

}
