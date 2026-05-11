using Godot;
using System;

namespace MechJamIV
{
    public interface IWeapon
    {

        //[Signal]
        delegate void FiredEventHandler();

        //[Signal]
        delegate void AmmoAddedEventHandler();

        PickupType WeaponType { get; }

        float RoundsPerSecond { get; }

        int Ammo { get; }

        //[Export(PropertyHint.Layers2DPhysics)]
        uint CollisionMask { get; }

        float LineOfSightDistance { get; }

        Texture2D UISprite { get; }

        void Fire(Vector2 globalPos, PhysicsBody2D target = null);

        void AddAmmo(int count);

    }
}