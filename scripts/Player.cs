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
	public float ThrowStrength { get; set; } = 500.0f;

	#region Node references

	public Marker2D RobotMarker { get; set; }

	private Timer immunityTimer;
	private GpuParticles2D immunityShield;
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
		hitScanBulletEmitter = GetNode<HitScanBulletEmitter>("HitScanBulletEmitter");
		camera2D = GetNode<Camera2D>("Camera2D");
    }

	protected override Vector2 GetMovementDirection() => Input.GetVector("move_left", "move_right", "move_up", "move_down");

    protected override bool IsJumping() => Input.IsActionJustPressed("jump") && IsOnFloor();

	protected override AttackType? IsAttacking()
	{
		// NOTE: We check for grenades first so player can hold down main fire button without interruption.

		if (Input.IsActionJustPressed("throw_grenade"))
		{
			return AttackType.Explosive;
		}
		else if (Input.IsActionJustPressed("fire"))
		{
			return AttackType.Gunfire;
		}
		else if (Input.IsActionPressed("fire"))
		{
			return AttackType.SustainedGunfire;
		}

		return null;
	}

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

    protected override void ProcessAttack(double delta, AttackType attackType)
    {
		switch (attackType)
		{
			case AttackType.Gunfire:
			case AttackType.SustainedGunfire:
				hitScanBulletEmitter.Fire(GetRelativeMousePosition());

				break;
			case AttackType.Explosive:
				Grenade grenade = grenadeResource.Instantiate<Grenade>();
				GetParent<World>().AddChild(grenade);

				grenade.GlobalTransform = GlobalTransform;

				grenade.ApplyImpulse(ThrowStrength * GetRelativeMousePosition().Normalized());

				break;
		}
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
