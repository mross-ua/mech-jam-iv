using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class WeaponManager : Node2D
{

	[Signal]
	public delegate void WeaponFiredEventHandler(FireMode fireMode, WeaponBase weapon);

	#region Node references

	public WeaponBase PrimaryWeapon { get; private set; }
	public WeaponBase SecondaryWeapon { get; private set; }

	#endregion

    public override void _Ready()
    {
		PrimaryWeapon = GetNodeOrNull<WeaponBase>("HitScanBulletEmitter");
		SecondaryWeapon = GetNodeOrNull<WeaponBase>("ProjectileEmitter");

		if (PrimaryWeapon != null)
		{
			PrimaryWeapon.Fired += () => EmitSignal(SignalName.WeaponFired, (long)FireMode.Primary, PrimaryWeapon);
		}

		if (SecondaryWeapon != null)
		{
			SecondaryWeapon.Fired += () => EmitSignal(SignalName.WeaponFired, (long)FireMode.Secondary, SecondaryWeapon);
		}
    }

	public void SetBodiesToExclude(IEnumerable<CollisionObject2D> bodies)
	{
		PrimaryWeapon?.SetBodiesToExclude(bodies);
		SecondaryWeapon?.SetBodiesToExclude(bodies);
	}

	public void Fire(FireMode mode, Vector2 globalPos, CollisionObject2D target = null)
	{
		switch (mode)
		{
			case FireMode.Primary:
			case FireMode.PrimarySustained:
				PrimaryWeapon?.Fire(globalPos, target);

				break;
			case FireMode.Secondary:
				SecondaryWeapon?.Fire(globalPos, target);

				break;
		}
	}

	public void PickupAmmo(PickupType pickupType)
	{
		switch (pickupType)
		{
			case PickupType.Grenade:
				if (SecondaryWeapon != null)
				{
					SecondaryWeapon.Ammo++;
				}

				break;
			case PickupType.Missile:
				if (SecondaryWeapon != null)
				{
					SecondaryWeapon.Ammo++;
				}

				break;
		}
	}

}
