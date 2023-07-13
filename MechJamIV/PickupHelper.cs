using Godot;
using System;
using System.Collections.Generic;

namespace MechJamIV {
    public static class PickupHelper
    {

        #region Resources

    	private static PackedScene medkitPickup = ResourceLoader.Load<PackedScene>("res://scenes/medkit_pickup.tscn");

    	private static PackedScene grenadePickup = ResourceLoader.Load<PackedScene>("res://scenes/grenade_pickup.tscn");

        #endregion

        public static PickupBase GenerateRandomPickup(float probability)
        {
            if (RandomHelper.GetSingle() <= probability)
            {
                switch ((PickupType)RandomHelper.GetInt(2))
                {
                    case PickupType.Medkit:
                        return medkitPickup.Instantiate<PickupBase>();

                    case PickupType.Grenade:
                        return grenadePickup.Instantiate<PickupBase>();
                }
            }

            return null;
        }

    }
}