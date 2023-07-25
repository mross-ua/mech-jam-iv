using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class WeaponManager : Node2D
{

	#region Node references

	private HitScanBulletEmitter hitScanBulletEmitter;
	private ProjectileEmitter projectileEmitter;

	#endregion

    public override void _Ready()
    {
		hitScanBulletEmitter = GetNodeOrNull<HitScanBulletEmitter>("HitScanBulletEmitter");
		projectileEmitter = GetNodeOrNull<ProjectileEmitter>("ProjectileEmitter");
    }

	public void SetBodiesToExclude(IEnumerable<PhysicsBody2D> bodies)
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
		if (projectileEmitter != null)
		{
			if (pickupType == PickupType.Grenade)
			{
				projectileEmitter.Ammo++;
			}
		}
	}

}
