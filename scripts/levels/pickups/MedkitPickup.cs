using Godot;
using System;
using MechJamIV;

public partial class MedkitPickup : PickupBase
{

	#region Node references

	private Projectile medkit;

	#endregion

	public override void _Ready()
	{
		base._Ready();

		// NOTE: In order to get the RemoteTransform2D to work, I had to
		//       set Medkit.TopLevel = true. Therefore, we have to set
		//       the initial position.

		medkit = GetNode<Projectile>("Medkit");
		medkit.GlobalPosition = GlobalPosition;
	}

}
