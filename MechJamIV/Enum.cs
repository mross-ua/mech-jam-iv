using Godot;
using System;

namespace MechJamIV {
    public enum CollisionLayer
    {
        World,
        Player,
        Robot,
        Environment,
        Hazard,
        Hitbox,
        Enemy
    }

    [Flags]
    public enum CollisionLayerMask : uint
    {
        World,
        Player,
        Robot,
        Environment,
        Hazard,
        Hitbox,
        Enemy
    }
}
