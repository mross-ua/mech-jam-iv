using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MechJamIV;

public partial class Player : CharacterBase
	,IPlayable
{

	#region Node references

	public Marker2D RobotMarker { get; private set; }

	private Timer immunityTimer;
	private GpuParticles2D immunityShield;
	private RemoteTransform2D remoteTransform;
	private WeaponManager weaponManager;

	#endregion

	#region Resources

	private static readonly PackedScene bloodSplatterResource = ResourceLoader.Load<PackedScene>("res://scenes/effects/blood_splatter.tscn");

	#endregion

    public override void _Ready()
    {
		base._Ready();

		RobotMarker = GetNode<Marker2D>("RobotMarker");

		immunityTimer = GetNode<Timer>("ImmunityTimer");
		immunityTimer.Timeout += () => DeactivateShield();

		immunityShield = GetNode<GpuParticles2D>("ImmunityShield");

		remoteTransform = GetNode<RemoteTransform2D>("RemoteTransform");

		weaponManager = GetNode<WeaponManager>("WeaponManager");
		weaponManager.SetBodiesToExclude(this.Yield());
    }

	protected override Vector2 GetMovementDirection()
	{
		Vector2 dir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		return new Vector2(dir.X, 0.0f).Normalized();
	}

    protected override bool IsJumping() => Input.IsActionJustPressed("jump") && IsOnFloor();

	public void Fire(FireMode mode, Vector2 globalPos)
	{
		if (Health <= 0)
		{
			return;
		}

		//TODO let's have missiles scan for the nearest target (long-term solution is to let user select a target)

		//CharacterBase c = GetTree().GetNodesInGroup("enemy").OfType<EnemyBase>().Where(e => e.Health > 0).FirstOrDefault();
		//weaponManager.Fire(mode, globalPos, c);
		weaponManager.Fire(mode, globalPos);
	}

	public void Pickup(PickupBase pickup)
	{
		switch (pickup.PickupType)
		{
			case PickupType.Medkit:
				Heal(50);

				break;
			case PickupType.Grenade:
				weaponManager.PickupAmmo(PickupType.Grenade);

				break;
		}
	}

	protected override void AnimateInjury(int damage, Vector2 globalPos, Vector2 normal)
    {
        this.EmitParticlesOnce(bloodSplatterResource.Instantiate<GpuParticles2D>(), globalPos);
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

	#region ICollidable

	public override void Hurt(int damage, Vector2 globalPos, Vector2 normal)
	{
		if (immunityTimer.TimeLeft > 0)
		{
			return;
		}

		base.Hurt(damage, globalPos, normal);

		if (Health <= 0)
		{
			return;
		}

		ActivateShield();

		immunityTimer.Start();
	}

	#endregion

	#region IPlayable

	[Signal]
	public delegate void ImmunityShieldActivatedEventHandler();
	[Signal]
	public delegate void ImmunityShieldDeactivatedEventHandler();

	public void SetRemoteTarget(PlayerCamera cam)
	{
		remoteTransform.RemotePath = cam.GetPath();
	}

	#endregion

}
