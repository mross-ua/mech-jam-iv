using Godot;
using System;

public partial class Robot : CharacterBody2D
{

	[Export]
	public float Speed { get; set; } = 300.0f;

	private Player player;

	private AnimatedSprite2D animatedSprite2D;

    public override void _Ready()
    {
		player = (Player)GetTree().GetFirstNodeInGroup("player");

		GlobalPosition = player.RobotMarker.GlobalPosition;

        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		Vector2 vectorToMarker = player.RobotMarker.GlobalTransform.Origin - GlobalTransform.Origin;

		if (vectorToMarker.IsZeroApprox())
		{
			animatedSprite2D.Play("idle");
		}
		else if (vectorToMarker.Length() < Speed)
		{
			velocity = vectorToMarker;
		}
		else
		{
			velocity = vectorToMarker.Normalized() * Speed;

			animatedSprite2D.Play("idle");
		}

		Velocity = velocity;

		MoveAndSlide();
	}

}
