using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class WeaponManager : Node2D
{

	[Signal]
	public delegate void WeaponFiredEventHandler(FireMode fireMode, int ammoRemaining);

	#region Node references

	private HitScanBulletEmitter hitScanBulletEmitter;
	private ProjectileEmitter projectileEmitter;

	#endregion

    public override void _Ready()
    {
		hitScanBulletEmitter = GetNodeOrNull<HitScanBulletEmitter>("HitScanBulletEmitter");
		projectileEmitter = GetNodeOrNull<ProjectileEmitter>("ProjectileEmitter");

		if (hitScanBulletEmitter != null)
		{
			hitScanBulletEmitter.Fired += (ammoRemaining) => EmitSignal(SignalName.WeaponFired, (long)FireMode.Primary, ammoRemaining);
		}

		if (projectileEmitter != null)
		{
			projectileEmitter.Fired += (ammoRemaining) => EmitSignal(SignalName.WeaponFired, (long)FireMode.Secondary, ammoRemaining);
		}
    }

	public void SetBodiesToExclude(IEnumerable<CollisionObject2D> bodies)
	{
		hitScanBulletEmitter?.SetBodiesToExclude(bodies);
		projectileEmitter?.SetBodiesToExclude(bodies);
	}

	public void Fire(FireMode mode, Vector2 globalPos, CharacterBase target = null)
	{
		switch (mode)
		{
			case FireMode.Primary:
				hitScanBulletEmitter?.Fire(globalPos);

				break;
			case FireMode.PrimarySustained:
				hitScanBulletEmitter?.Fire(globalPos);

				break;
			case FireMode.Secondary:
				projectileEmitter?.Fire(globalPos, target);

				break;
		}
	}

	public void PickupAmmo(PickupType pickupType)
	{
		switch (pickupType)
		{
			case PickupType.Grenade:
				if (projectileEmitter != null)
				{
					projectileEmitter.Ammo++;
				}

				break;
		}
	}

}
