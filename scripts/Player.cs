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

	protected override Vector2 GetMovementDirection()
	{
		Vector2 dir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		return new Vector2(dir.X, 0.0f).Normalized();
	}

    protected override bool IsJumping() => Input.IsActionJustPressed("jump") && IsOnFloor();

	private Vector2 GetRelativeMousePosition()
	{
		//return GetViewport().GetMousePosition();
		//return GetGlobalMousePosition();
		//TODO why does this work?
		return camera2D.GetGlobalMousePosition() - GlobalTransform.Origin;
	}

#if DEBUG
    public override void _Draw()
    {
     	base._Draw();

		DrawLine(Vector2.Zero, GetRelativeMousePosition(), Colors.Green, 1.0f);
    }
#endif

    protected override void ProcessAction()
    {
		if (attackTimer.TimeLeft > 0)
		{
			return;
		}

		// NOTE: We check for grenades first so player can hold down main fire button without interruption.

		if (Input.IsActionJustPressed("throw_grenade") && GrenadeCount > 0)
		{
			Grenade grenade = grenadeResource.Instantiate<Grenade>();
			GetTree().CurrentScene.AddChild(grenade);

			grenade.GlobalTransform = GlobalTransform;

			grenade.ApplyImpulse(ThrowStrength * GetRelativeMousePosition().Normalized());

			grenade.Prime();

			GrenadeCount--;

			attackTimer.Start();
		}
		//TODO?
		// else if (Input.IsActionJustPressed("fire"))
		// {
		// 	hitScanBulletEmitter.Fire(GetRelativeMousePosition());
		//
		//  attackTimer.Start();
		// }
		else if (Input.IsActionPressed("fire"))
		{
			hitScanBulletEmitter.Fire(GetRelativeMousePosition());

			attackTimer.Start();
		}
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
