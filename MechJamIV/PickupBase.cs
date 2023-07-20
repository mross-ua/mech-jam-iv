using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

namespace MechJamIV {
	public abstract partial class PickupBase : Area2D
	{

        [Signal]
        public delegate void PickedUpEventHandler();

        public abstract PickupType PickupType { get; protected set; }

        public override void _Ready()
        {
            BodyEntered += (body) =>
            {
                EmitSignal(SignalName.PickedUp);

                QueueFree();
            };
        }

	}
}
