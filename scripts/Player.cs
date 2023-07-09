using Godot;
using System;
using MechJamIV;

public partial class Player : CharacterBase
{

	[Signal]
	public delegate void ImmunityShieldActivatedEventHandler();
	[Signal]
	public delegate void ImmunityShieldDeactivatedEventHandler();

	private SceneTreeTimer immunityTimer = null;

	#region Node references

	public Marker2D RobotMarker;
	private AnimatedSprite2D animatedSprite2D;
	private GpuParticles2D immunityShield;
	private HitScanBulletEmitter hitScanBulletEmitter;

	#endregion

    public override void _Ready()
    {
		base._Ready();

		RobotMarker = GetNode<Marker2D>("RobotMarker");
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		immunityShield = GetNode<GpuParticles2D>("ImmunityShield");

		hitScanBulletEmitter = GetNode<HitScanBulletEmitter>("HitScanBulletEmitter");
    }

	protected override Vector2 GetMovementDirection(double delta)
	{
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

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

    protected override bool IsJumping()
    {
        return IsOnFloor() && Input.IsActionJustPressed("jump");
    }

	protected override bool IsAttacking() => Input.IsActionPressed("fire");

    public override void _PhysicsProcess(double delta)
    {
		if (Health <= 0)
		{
			return;
		}

		if (IsAttacking())
		{
			hitScanBulletEmitter.Fire();
		}

        base._PhysicsProcess(delta);
    }

	public override async void HurtAsync(int damage, Vector2 normal)
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

		ActivateShield();

		base.HurtAsync(damage, normal);

		if (Health <= 0)
		{
			animatedSprite2D.Play("death");

			await ToSignal(GetTree().CreateTimer(5.0f), SceneTreeTimer.SignalName.Timeout);

			//TODO be careful with this (it messes up the camera and probably the robot too)
			//QueueFree();
		}

		await ToSignal(immunityTimer, SceneTreeTimer.SignalName.Timeout);

		DeactivateShield();

		immunityTimer = null;
	}

	public void ActivateShield()
	{
		immunityShield.Visible = true;

		EmitSignal(SignalName.ImmunityShieldActivated);
	}

	public void DeactivateShield()
	{
		immunityShield.Visible = false;

		EmitSignal(SignalName.ImmunityShieldDeactivated);
	}

}
