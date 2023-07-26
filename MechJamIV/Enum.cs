using Godot;
using System;

namespace MechJamIV {
    public enum CollisionLayer : long
    {
        World,
        Player,
        Robot,
        Environment,
        Hazard,
        Hitbox,
        Enemy,
        Objective,
        Projectile
    }

    [Flags]
    public enum CollisionLayerMask : uint
    {
        World = 1,
        Player = 2,
        Robot = 4,
        Environment = 8,
        Hazard = 16,
        Hitbox = 32,
        Enemy = 64,
        Objective = 128,
        Projectile = 256
    }

    public enum PickupType : long
    {
        Medkit = 0,
        Grenade = 1
    }

    public enum EnemyState : long
    {
        Idle,
        Chase,
        Attacking
    }

    public enum FireMode : long
    {
        Primary,
        PrimarySustained,
        Secondary
    }
}
