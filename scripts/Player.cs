using Godot;
using System;
using MechJamIV;

public partial class Player : CharacterBase
{

	[Signal]
	public delegate void ImmunityShieldActivatedEventHandler();
	[Signal]
	public delegate void ImmunityShieldDeactivatedEventHandler();

    public override Vector2 FaceDirection { get; set; } = Vector2.Right;

	[Export]
	public int GrenadeCount { get; set; } = 4;

	[Export]
	public float ThrowStrength { get; set; } = 500.0f;

	#region Node references

	public Marker2D RobotMarker { get; set; }
	public RemoteTransform2D RemoteTransform { get; set; }
	public HitScanBulletEmitter HitScanBulletEmitter { get; private set; }

	private Timer immunityTimer;
	private GpuParticles2D immunityShield;
	private Timer attackTimer;

	#endregion

	#region Resources

	private PackedScene grenadeResource = ResourceLoader.Load<PackedScene>("res://scenes/weapons/grenade.tscn");
	private PackedScene bloodSplatterResource = ResourceLoader.Load<PackedScene>("res://scenes/effects/blood_splatter.tscn");

	#endregion

    public override void _Ready()
    {
		base._Ready();

		RobotMarker = GetNode<Marker2D>("RobotMarker");
		RemoteTransform = GetNode<RemoteTransform2D>("RemoteTransform");

		immunityTimer = GetNode<Timer>("ImmunityTimer");
		immunityTimer.Timeout += () => DeactivateShield();

		immunityShield = GetNode<GpuParticles2D>("ImmunityShield");
		attackTimer = GetNode<Timer>("AttackTimer");
		HitScanBulletEmitter = GetNode<HitScanBulletEmitter>("HitScanBulletEmitter");
    }

	protected override Vector2 GetMovementDirection()
	{
		Vector2 dir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		return new Vector2(dir.X, 0.0f).Normalized();
	}

    protected override bool IsJumping() => Input.IsActionJustPressed("jump") && IsOnFloor();

	public void FireGun(Vector2 direction)
	{
		if (Health <= 0)
		{
			return;
		}
		else if (attackTimer.TimeLeft > 0)
		{
			return;
		}

		HitScanBulletEmitter.Fire(direction);

		attackTimer.Start();
	}

	public void ThrowGrenade(Vector2 direction)
	{
		if (Health <= 0)
		{
			return;
		}
		else if (attackTimer.TimeLeft > 0)
		{
			return;
		}
		else if (GrenadeCount <= 0)
		{
			return;
		}

		Grenade grenade = grenadeResource.Instantiate<Grenade>();
		GetTree().CurrentScene.AddChild(grenade);

		grenade.GlobalTransform = GlobalTransform;

		grenade.ApplyImpulse(ThrowStrength * direction);

		grenade.Prime();

		GrenadeCount--;

		attackTimer.Start();
	}

	public override void Hurt(int damage, Vector2 position, Vector2 normal)
	{
		if (immunityTimer.TimeLeft > 0)
		{
			return;
		}

		base.Hurt(damage, position, normal);

		if (Health <= 0)
		{
			return;
		}

		ActivateShield();

		immunityTimer.Start();
	}

	protected override void AnimateInjury(int damage, Vector2 position, Vector2 normal)
    {
        GpuParticles2D splatter = bloodSplatterResource.Instantiate<GpuParticles2D>();

		GetTree().CurrentScene.AddChild(splatter);

		splatter.GlobalPosition = position;

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
