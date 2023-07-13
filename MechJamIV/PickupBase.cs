using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

namespace MechJamIV {
	public abstract partial class PickupBase : Area2D
	{

        [Signal]
        public delegate void PickedUpEventHandler(PickupType pickupType);

        protected abstract PickupType PickupType { get; }

        public override void _Ready()
        {
            BodyEntered += (body) =>
            {
                EmitSignal(SignalName.PickedUp, (int)PickupType);

                this.QueueFree();
            };
        }

	}
}
