using Godot;
using System;

public partial class Player : CharacterBody2D
{

	[Signal]
	public delegate void HealEventHandler(int hp);
	[Signal]
	public delegate void HurtEventHandler(int damage);

	[Export]
	public int Health { get; set; } = 100;
	[Export]
	public float Speed { get; set; } = 300.0f;
	[Export]
	public float JumpVelocity { get; set; } = -400.0f;

	public Marker2D RobotMarker;

	private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private AnimatedSprite2D animatedSprite2D;
	private GpuParticles2D immunityShield;

	private SceneTreeTimer immunityTimer = null;

    public override void _Ready()
    {
		RobotMarker = GetNode<Marker2D>("RobotMarker");
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		immunityShield = GetNode<GpuParticles2D>("ImmunityShield");
    }

	public override void _PhysicsProcess(double delta)
	{
		if (Health <= 0)
		{
			return;
		}

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

	public async void HurtAsync(int damage)
	{
		if (immunityTimer != null)
		{
			return;
		}
		else if (Health <= 0)
		{
			return;
		}

		immunityTimer = GetTree().CreateTimer(2.0f);

		immunityShield.Visible = true;

		Health = Math.Max(0, Health - damage);

		if (Health <= 0)
		{
			animatedSprite2D.Play("death", 0.2f);
		}

		EmitSignal(SignalName.Hurt, damage);

		await ToSignal(immunityTimer, SceneTreeTimer.SignalName.Timeout);

		immunityShield.Visible = false;

		immunityTimer = null;
	}

}
