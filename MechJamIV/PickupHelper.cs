using Godot;
using System;
using System.Collections.Generic;

namespace MechJamIV {
    public static class PickupHelper
    {

        #region Resources

    	private static readonly PackedScene medkitPickup = ResourceLoader.Load<PackedScene>("res://scenes/levels/pickups/medkit_pickup.tscn");

    	private static readonly PackedScene grenadePickup = ResourceLoader.Load<PackedScene>("res://scenes/levels/pickups/grenade_pickup.tscn");

    	private static readonly PackedScene missilePickup = ResourceLoader.Load<PackedScene>("res://scenes/levels/pickups/missile_pickup.tscn");
    	private static readonly PackedScene grenadeEmitter = ResourceLoader.Load<PackedScene>("res://scenes/weapons/grenade_emitter.tscn");
    	private static readonly PackedScene missileEmitter = ResourceLoader.Load<PackedScene>("res://scenes/weapons/missile_emitter.tscn");

        #endregion

        public static PickupBase GenerateRandomPickup(float probability)
        {
            if (RandomHelper.GetSingle() <= probability)
            {
                switch (RandomHelper.GetInt(3))
                {
                    case 0:
                        return medkitPickup.Instantiate<MedkitPickup>();

                    case 1:
                        return grenadePickup.Instantiate<GrenadePickup>();

                    case 2:
                         return missilePickup.Instantiate<MissilePickup>();
                }
            }

            return null;
        }

        public static WeaponBase GenerateWeapon(PickupType pickupType)
        {
            switch (pickupType)
            {
                case PickupType.Grenade:
                    return grenadeEmitter.Instantiate<ProjectileEmitter>();

                case PickupType.Missile:
                    return missileEmitter.Instantiate<ProjectileEmitter>();
            }

            return null;
        }

    }
}