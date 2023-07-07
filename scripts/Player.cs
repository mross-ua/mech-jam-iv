using Godot;
using System;

public partial class Player : CharacterBody2D
{

	[Export]
	public float Speed { get; set; } = 300.0f;
	[Export]
	public float JumpVelocity { get; set; } = -400.0f;

	public Marker2D RobotMarker;

	private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private AnimatedSprite2D animatedSprite2D;

    public override void _Ready()
    {
		RobotMarker = GetNode<Marker2D>("RobotMarker");
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (IsOnFloor() && Input.IsActionJustPressed("jump"))
		{
			velocity.Y = JumpVelocity;
		}
		else if (!IsOnFloor())
		{
			velocity.Y += gravity * (float)delta;
		}

		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		if (Mathf.IsZeroApprox(direction.X))
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);

			animatedSprite2D.Play("idle", 0.2f);
		}
		else
		{
			velocity.X = direction.X * Speed;

			animatedSprite2D.Play("run", 0.2f);
		}

		Velocity = velocity;

		MoveAndSlide();
	}

}
