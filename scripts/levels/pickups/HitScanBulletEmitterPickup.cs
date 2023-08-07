using Godot;
using System;
using MechJamIV;

public partial class HitScanBulletEmitterPickup : PickupBase
{

	#region Node references

	private HitScanBulletEmitter hitScanBulletEmitter;

	#endregion

	public override void _Ready()
	{
		base._Ready();

		// NOTE: In order to get the RemoteTransform2D to work, I had to
		//       set Grenade.TopLevel = true. Therefore, we have to set
		//       the initial position.

		hitScanBulletEmitter = GetNode<HitScanBulletEmitter>("HitScanBulletEmitter");
		hitScanBulletEmitter.GlobalPosition = GlobalPosition;
		hitScanBulletEmitter.GetNode<Sprite2D>("UISprite").Visible = true;
	}

}
