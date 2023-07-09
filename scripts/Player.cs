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
	private GpuParticles2D immunityShield;
	private HitScanBulletEmitter hitScanBulletEmitter;

	#endregion

    public override void _Ready()
    {
		base._Ready();

		RobotMarker = GetNode<Marker2D>("RobotMarker");
		immunityShield = GetNode<GpuParticles2D>("ImmunityShield");

		hitScanBulletEmitter = GetNode<HitScanBulletEmitter>("HitScanBulletEmitter");
    }

	protected override Vector2 GetMovementDirection() => Input.GetVector("move_left", "move_right", "move_up", "move_down");

    protected override bool IsJumping() => Input.IsActionJustPressed("jump") && IsOnFloor();

	protected override bool IsAttacking() => Input.IsActionPressed("fire");

    protected override void ProcessAttack(double delta)
    {
		hitScanBulletEmitter.Fire();
    }

	public override async void HurtAsync(int damage, Vector2 normal)
	{
		if (immunityTimer != null)
		{
			return;
		}

		base.HurtAsync(damage, normal);

		if (Health <= 0)
		{
			return;
		}

		immunityTimer = GetTree().CreateTimer(2.0f);

		ActivateShield();

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
