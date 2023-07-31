using Godot;
using System;
using MechJamIV;

public partial class GrenadePickup : PickupBase
{

	#region Node references

	private Grenade grenade;

	#endregion

	public override void _Ready()
	{
		base._Ready();

		// NOTE: In order to get the RemoteTransform2D to work, I had to
		//       set Grenade.TopLevel = true. Therefore, we have to set
		//       the initial position.

		grenade = GetNode<Grenade>("Grenade");
		grenade.GlobalPosition = GlobalPosition;
		grenade.Killed += () => QueueFree();
	}

}
