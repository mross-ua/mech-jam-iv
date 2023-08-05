using Godot;
using System;
using MechJamIV;

public partial class MissilePickup : PickupBase
{

	#region Node references

	private Missile missile;

	#endregion

	public override void _Ready()
	{
		base._Ready();

		// NOTE: In order to get the RemoteTransform2D to work, I had to
		//       set Missile.TopLevel = true. Therefore, we have to set
		//       the initial position.

		missile = GetNode<Missile>("Missile");
		missile.GlobalPosition = GlobalPosition;
		missile.Killed += () => QueueFree();
	}

}
