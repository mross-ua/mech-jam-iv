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

    private IEnumerable<PhysicsBody2D> bodiesToExclude = null;

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

        NextWeaponPrimary();
        NextWeaponSecondary();
    }

    private void InitWeapon(WeaponBase weapon)
    {
        weapon.SetBodiesToExclude(bodiesToExclude);

        weapon.Fired += () => EmitSignal(SignalName.WeaponUpdated, weapon);
        weapon.AmmoAdded += () => EmitSignal(SignalName.WeaponUpdated, weapon);

        weapons[weapon.WeaponType] = weapon;
    }

    public void SetBodiesToExclude(IEnumerable<PhysicsBody2D> bodies)
    {
        bodiesToExclude = bodies == null ? null : new List<PhysicsBody2D>(bodies);

        foreach (WeaponBase weapon in weapons.Values)
        {
            weapon.SetBodiesToExclude(bodies);
        }
    }

    public void Fire(FireMode mode, Vector2 globalPos, PhysicsBody2D target = null)
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

    public async void Pickup(PickupType pickupType)
    {
        if (!weapons.ContainsKey(pickupType))
        {
            WeaponBase weapon = PickupHelper.GenerateWeapon(pickupType);

            InitWeapon(weapon);

            await this.AddChildDeferred(weapon);

            // auto-select the weapon if needed

            if (PrimaryWeapon == null)
            {
                NextWeaponPrimary();
            }

            if (SecondaryWeapon == null)
            {
                NextWeaponSecondary();
            }
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

    public void NextWeaponPrimary()
    {
        bool isWeaponFound = false;

        WeaponBase firstWeapon = null;
        WeaponBase lastWeapon = null;

        foreach (WeaponBase weapon in weapons.Values)
        {
            switch (weapon.WeaponType)
            {
                case PickupType.Rifle:
                    if (PrimaryWeapon == null || isWeaponFound)
                    {
                        PrimaryWeapon = weapon;

                        EmitSignal(SignalName.WeaponUpdated, weapon);

                        return;
                    }

                    firstWeapon ??= weapon;
                    lastWeapon = weapon;

                    isWeaponFound = (PrimaryWeapon == weapon);

                    break;
                default:
                    // ignore

                    break;
            }
        }

        if (isWeaponFound && firstWeapon != lastWeapon)
        {
            PrimaryWeapon = firstWeapon;

            EmitSignal(SignalName.WeaponUpdated, firstWeapon);
        }
    }

    public void NextWeaponSecondary()
    {
        bool isWeaponFound = false;

        WeaponBase firstWeapon = null;
        WeaponBase lastWeapon = null;

        foreach (WeaponBase weapon in weapons.Values)
        {
            switch (weapon.WeaponType)
            {
                case PickupType.Grenade:
                case PickupType.Missile:
                    if (SecondaryWeapon == null || isWeaponFound)
                    {
                        SecondaryWeapon = weapon;

                        EmitSignal(SignalName.WeaponUpdated, weapon);

                        return;
                    }

                    firstWeapon ??= weapon;
                    lastWeapon = weapon;

                    isWeaponFound = (SecondaryWeapon == weapon);

                    break;
                default:
                    // ignore

                    break;
            }
        }

        if (isWeaponFound && firstWeapon != lastWeapon)
        {
            SecondaryWeapon = firstWeapon;

            EmitSignal(SignalName.WeaponUpdated, firstWeapon);
        }
    }

}
