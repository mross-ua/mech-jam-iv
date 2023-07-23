public partial class CharacterAnimator : AnimatedSprite2D
{

	[Export]
	public Vector2 SpriteFaceDirection { get; set; } = Vector2.Zero;

	public void AnimateMovement(Vector2 direction)
	{
		if (Mathf.IsZeroApprox(direction.X))
		{
			Play("idle");
		}
		else
		{
			Play("run");

			FlipH = !SpriteFaceDirection.IsEqualApprox(direction);
		}
	}

	public void AnimateDeath()
	{
		Play("death");
	}

}
