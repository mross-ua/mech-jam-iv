using Godot;
using System;
using MechJamIV;

public partial class WeaponManager : Node2D
{

	#region Node references

	private HitScanBulletEmitter hitScanBulletEmitter;
	private ProjectileEmitter projectileEmitter;

	#endregion

    public override void _Ready()
    {
		hitScanBulletEmitter = GetNode<HitScanBulletEmitter>("HitScanBulletEmitter");
		projectileEmitter = GetNode<ProjectileEmitter>("ProjectileEmitter");
    }

	public void Fire(FireMode mode, Vector2 globalPos)
	{
		switch (mode)
		{
			case FireMode.Primary:
				hitScanBulletEmitter.Fire(globalPos);

				break;
			case FireMode.PrimarySustained:
				hitScanBulletEmitter.Fire(globalPos);

				break;
			case FireMode.Secondary:
				projectileEmitter.Fire(globalPos);

				break;
		}
	}

	public void PickupAmmo(PickupType pickupType)
	{
		if (pickupType == PickupType.Grenade)
		{
			projectileEmitter.Ammo++;
		}
	}

}
