using Godot;
using System;

public partial class CharacterAnimator : AnimatedSprite2D
{

	public void AnimateMovement(Vector2 direction)
	{
		if (Mathf.IsZeroApprox(direction.X))
		{
			Play("idle");
		}
		else
		{
			Play("run");

			FlipH = direction.X < 0.0f;
		}
	}

	public void AnimateDeath()
	{
		Play("death");
	}

}
