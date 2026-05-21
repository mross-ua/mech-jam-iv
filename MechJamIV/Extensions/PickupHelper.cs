using Godot;
using MechJamIV.Base;
using MechJamIV.Enums;
using System;

namespace MechJamIV.Extensions
{
    public static class PickupHelper
    {

        public static PickupType? GenerateRandomPickup(float probability)
        {
            if (GD.Randf() <= probability)
            {
                switch (GD.Randi() % 3)
                {
                    case 0:
                        return PickupType.Medkit;

                    case 1:
                        return PickupType.Grenade;

                    case 2:
                        return PickupType.Missile;
                }
            }

            return null;
        }

        public static Projectile GenerateProjectile(PickupType pickupType)
        {
            switch (pickupType)
            {
                case PickupType.Medkit:
                    return ResourceManager.Medkit.Instantiate<Projectile>();

                case PickupType.Grenade:
                    return ResourceManager.Grenade.Instantiate<Projectile>();

                case PickupType.Missile:
                    return ResourceManager.Missile.Instantiate<Missile>();
            }

            throw new ArgumentException($"Pickup type {Enum.GetName(pickupType)} is not a dropped pickup type");
        }

        public static WeaponBase GenerateWeapon(PickupType pickupType)
        {
            switch (pickupType)
            {
                case PickupType.Rifle:
                    return ResourceManager.HitScanBulletEmitter.Instantiate<HitScanBulletEmitter>();

                case PickupType.Grenade:
                    return ResourceManager.GrenadeEmitter.Instantiate<ProjectileEmitter>();

                case PickupType.Missile:
                    return ResourceManager.MissileEmitter.Instantiate<ProjectileEmitter>();
            }

            throw new ArgumentException($"Pickup type {Enum.GetName(pickupType)} is not a weapon type");
        }

    }
}