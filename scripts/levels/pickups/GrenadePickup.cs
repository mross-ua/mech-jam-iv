using Godot;
using System;
using MechJamIV;

public partial class GrenadePickup : PickupBase
{

	#region Node references

	private ExplosiveProjectile grenade;

	#endregion

	public override void _Ready()
	{
		base._Ready();

		// NOTE: In order to get the RemoteTransform2D to work, I had to
		//       set Grenade.TopLevel = true. Therefore, we have to set
		//       the initial position.

		grenade = GetNode<ExplosiveProjectile>("Grenade");
		grenade.GlobalPosition = GlobalPosition;
		grenade.Killed += () => QueueFree();
	}

}
