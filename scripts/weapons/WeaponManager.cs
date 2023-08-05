using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MechJamIV;

public partial class WeaponManager : Node2D
{

	[Signal]
	public delegate void WeaponUpdatedEventHandler(WeaponBase weapon);

	private Dictionary<PickupType, WeaponBase> weapons;

    private IEnumerable<CollisionObject2D> bodiesToExclude = null;

	#region Node references

	public WeaponBase PrimaryWeapon { get; private set; }
	public WeaponBase SecondaryWeapon { get; private set; }

	#endregion

    public override void _Ready()
    {
		InitWeapons();
    }

	private void InitWeapons()
	{
		weapons = new Dictionary<PickupType, WeaponBase>();

		foreach (WeaponBase weapon in GetChildren().Where(n => n.IsInGroup("weapon")).OfType<WeaponBase>())
		{
			InitWeapon(weapon);
		}
	}

	private void InitWeapon(WeaponBase weapon)
	{
		weapon.SetBodiesToExclude(bodiesToExclude);

		weapon.Fired += () => EmitSignal(SignalName.WeaponUpdated, weapon);
		weapon.AmmoAdded += () => EmitSignal(SignalName.WeaponUpdated, weapon);

		weapons[weapon.WeaponType] = weapon;

		switch (weapon.WeaponType)
		{
			case PickupType.Rifle:
				if (PrimaryWeapon == null)
				{
					PrimaryWeapon = weapon;

					EmitSignal(SignalName.WeaponUpdated, weapon);
				}

				break;
			case PickupType.Grenade:
			case PickupType.Missile:
				if (SecondaryWeapon == null)
				{
					SecondaryWeapon = weapon;

					EmitSignal(SignalName.WeaponUpdated, weapon);
				}

				break;
		}
	}

	public void SetBodiesToExclude(IEnumerable<CollisionObject2D> bodies)
	{
		if (bodies == null)
		{
			bodiesToExclude = null;
		}
		else
		{
			bodiesToExclude = new List<CollisionObject2D>(bodies);
		}

		foreach (WeaponBase weapon in weapons.Values)
		{
			weapon.SetBodiesToExclude(bodies);
		}
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

	public void Pickup(PickupType pickupType)
	{
		if (!weapons.ContainsKey(pickupType))
		{
			WeaponBase weapon = PickupHelper.GenerateWeapon(pickupType);

			InitWeapon(weapon);

			//TODO? await this.AddChildDeferred(weapon);
			AddChild(weapon);
		}

		switch (pickupType)
		{
			case PickupType.Rifle:
				weapons[pickupType].AddAmmo(30);

				break;
			case PickupType.Grenade:
			case PickupType.Missile:
				weapons[pickupType].AddAmmo(1);

				break;
		}
	}

}
