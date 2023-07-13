using Godot;
using System;
using MechJamIV;

public partial class Player : CharacterBase
{

	[Signal]
	public delegate void ImmunityShieldActivatedEventHandler();
	[Signal]
	public delegate void ImmunityShieldDeactivatedEventHandler();

	[Export]
	public int GrenadeCount { get; set; } = 4;

	[Export]
	public float ThrowStrength { get; set; } = 500.0f;

	#region Node references

	public Marker2D RobotMarker { get; set; }

	private Timer immunityTimer;
	private GpuParticles2D immunityShield;
	private Timer attackTimer;
	private HitScanBulletEmitter hitScanBulletEmitter;
	//TODO remove! this is temporary!
	private Camera2D camera2D;

	#endregion

	#region Resources

	private PackedScene grenadeResource = ResourceLoader.Load<PackedScene>("res://scenes/grenade.tscn");
	private PackedScene bloodSplatterResource = ResourceLoader.Load<PackedScene>("res://scenes/blood_splatter.tscn");

	#endregion

    public override void _Ready()
    {
		base._Ready();

		RobotMarker = GetNode<Marker2D>("RobotMarker");

		immunityTimer = GetNode<Timer>("ImmunityTimer");
		immunityTimer.Timeout += () => DeactivateShield();

		immunityShield = GetNode<GpuParticles2D>("ImmunityShield");

		attackTimer = GetNode<Timer>("AttackTimer");

		hitScanBulletEmitter = GetNode<HitScanBulletEmitter>("HitScanBulletEmitter");
		camera2D = GetNode<Camera2D>("Camera2D");
    }

	protected override Vector2 GetMovementDirection() => Input.GetVector("move_left", "move_right", "move_up", "move_down");

    protected override bool IsJumping() => Input.IsActionJustPressed("jump") && IsOnFloor();

	private Vector2 GetRelativeMousePosition()
	{
		//return GetViewport().GetMousePosition();
		//return GetGlobalMousePosition();
		//TODO why does this work?
		return camera2D.GetGlobalMousePosition() - GlobalTransform.Origin;
	}

    // public override void _Draw()
    // {
    //     base._Draw();

	// 	DrawLine(Vector2.Zero, GetRelativeMousePosition(), Colors.Green, 1.0f);
    // }

    protected override void ProcessAttack(double delta)
    {
		if (attackTimer.TimeLeft > 0)
		{
			return;
		}

		// NOTE: We check for grenades first so player can hold down main fire button without interruption.

		if (Input.IsActionJustPressed("throw_grenade") && GrenadeCount > 0)
		{
			Grenade grenade = grenadeResource.Instantiate<Grenade>();
			GetTree().Root.AddChild(grenade);

			grenade.GlobalTransform = GlobalTransform;

			grenade.ApplyImpulse(ThrowStrength * GetRelativeMousePosition().Normalized());

			grenade.Prime();

			GrenadeCount--;
		}
		//TODO?
		// else if (Input.IsActionJustPressed("fire"))
		// {
		// 	hitScanBulletEmitter.Fire(GetRelativeMousePosition());
		// }
		else if (Input.IsActionPressed("fire"))
		{
			hitScanBulletEmitter.Fire(GetRelativeMousePosition());
		}

		attackTimer.Start();
    }

	public override void Hurt(int damage, Vector2 normal)
	{
		if (immunityTimer.TimeLeft > 0)
		{
			return;
		}

		base.Hurt(damage, normal);

		if (Health <= 0)
		{
			return;
		}

		ActivateShield();

		immunityTimer.Start();
	}

	protected override void AnimateInjury(int damage, Vector2 normal)
    {
        GpuParticles2D splatter = bloodSplatterResource.Instantiate<GpuParticles2D>();

		AddChild(splatter);

		splatter.Emitting = true;

		splatter.TimedFree(splatter.Lifetime + splatter.Lifetime * splatter.Randomness, processInPhysics:true);
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
