using Godot;
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

    }
}
