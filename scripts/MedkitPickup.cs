using Godot;
using System;
using MechJamIV;

public partial class MedkitPickup : PickupBase
{
	protected override PickupType PickupType => PickupType.Medkit;

	#region Node references

	private AnimatedSprite2D animatedSprite2D;

	#endregion

	public override void _Ready()
	{
		base._Ready();

		Random rnd = new Random((int)DateTime.Now.Ticks);

		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.AnimationLooped += () => animatedSprite2D.Rotate(Mathf.DegToRad(rnd.Next(360)));
	}

}
