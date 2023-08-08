using Godot;
using System;

namespace MechJamIV
{
    public interface IWeapon
    {

        //[Signal]
        public delegate void FiredEventHandler();

        //[Signal]
        public delegate void AmmoAddedEventHandler();

        public PickupType WeaponType { get; }

        public float RoundsPerSecond { get; }

        public int Ammo { get; }

        //[Export(PropertyHint.Layers2DPhysics)]
        public uint CollisionMask { get; }

        public float LineOfSightDistance { get; }

        public Texture2D UISprite { get; }

        public void Fire(Vector2 globalPos, PhysicsBody2D target = null);

        public void AddAmmo(int count);

    }
}