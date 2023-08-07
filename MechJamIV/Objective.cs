using Godot;
using System;

namespace MechJamIV {
	public partial class Objective : Area2D
	{

		[Signal]
		public delegate void ObjectiveReachedEventHandler();

		public override void _Ready()
		{
			BodyEntered += (body) => EmitSignal(SignalName.ObjectiveReached);
		}

	}
}
