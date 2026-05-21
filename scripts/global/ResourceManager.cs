using Godot;
using MechJamIV.Base;
using MechJamIV.Enums;
using System;

namespace MechJamIV
{
    public partial class ResourceManager : Node
    {

        private static ResourceManager Instance { get; set; } = null!;

        #region Resources

        public static readonly CompressedTexture2D CursorTexture = ResourceLoader.Load<CompressedTexture2D>("res://assets/sprites/WhiteCrosshair-5.png");

        public static readonly PackedScene Medkit = ResourceLoader.Load<PackedScene>("res://scenes/levels/pickups/medkit.tscn");
        public static readonly PackedScene Grenade = ResourceLoader.Load<PackedScene>("res://scenes/weapons/grenade.tscn");
        public static readonly PackedScene Missile = ResourceLoader.Load<PackedScene>("res://scenes/weapons/missile.tscn");

        public static readonly PackedScene HitScanBulletEmitter = ResourceLoader.Load<PackedScene>("res://scenes/weapons/hit_scan_bullet_emitter.tscn");
        public static readonly PackedScene GrenadeEmitter = ResourceLoader.Load<PackedScene>("res://scenes/weapons/grenade_emitter.tscn");
        public static readonly PackedScene MissileEmitter = ResourceLoader.Load<PackedScene>("res://scenes/weapons/missile_emitter.tscn");

        #endregion

        public override void _Ready()
        {
            Instance ??= this;
        }

        public static Projectile GenerateProjectile(PickupType pickupType)
        {
            switch (pickupType)
            {
                case PickupType.Medkit:
                    return Medkit.Instantiate<Projectile>();

                case PickupType.Grenade:
                    return Grenade.Instantiate<Projectile>();

                case PickupType.Missile:
                    return Missile.Instantiate<Missile>();
            }

            throw new ArgumentException($"Pickup type {Enum.GetName(pickupType)} is not a dropped pickup type");
        }

        public static WeaponBase GenerateWeapon(PickupType pickupType)
        {
            switch (pickupType)
            {
                case PickupType.Rifle:
                    return HitScanBulletEmitter.Instantiate<HitScanBulletEmitter>();

                case PickupType.Grenade:
                    return GrenadeEmitter.Instantiate<ProjectileEmitter>();

                case PickupType.Missile:
                    return MissileEmitter.Instantiate<ProjectileEmitter>();
            }

            throw new ArgumentException($"Pickup type {Enum.GetName(pickupType)} is not a weapon type");
        }

    }
}
