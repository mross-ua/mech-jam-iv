using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

namespace MechJamIV {
	public abstract partial class PickupBase : Area2D
	{

        [Signal]
        public delegate void PickedUpEventHandler();

        [Export(PropertyHint.Enum)]
        public PickupType PickupType { get; set; }

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
