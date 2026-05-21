using Godot;
using MechJamIV.Base;
using MechJamIV.Enums;
using System;

namespace MechJamIV.Extensions
{
    public static class PickupHelper
    {

        public static PickupType? GetRandomPickup(float probability)
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

    }
}