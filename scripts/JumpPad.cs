using Godot;
using System;

public partial class JumpPad : Area2D
{

	[Export]
	public float JumpMultiplier { get; set; } = 2.0f;

	private AnimatedSprite2D animatedSprite2D;

	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.AnimationFinished += () =>
		{
			if (animatedSprite2D.Animation == "jump")
			{
				animatedSprite2D.Animation = "idle";
			}
		};

		BodyEntered += (body) =>
		{
			if (body is Player player && player.Velocity.Y > 0.0f)
			{
				player.Velocity = new Vector2(0.0f, player.JumpVelocity * JumpMultiplier);

				animatedSprite2D.Play("jump");
			}
		};
	}

	public override void _PhysicsProcess(double delta)
	{

	}

}
