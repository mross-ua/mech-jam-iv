using Godot;
using System;
using MechJamIV;

public partial class Medkit : Barrel
{

	#region Node references

	private AnimatedSprite2D animatedSprite2D;

	#endregion

	public override void _Ready()
	{
		base._Ready();

		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		// make it flicker
		animatedSprite2D.AnimationLooped += () => animatedSprite2D.Rotate(Mathf.DegToRad(RandomHelper.GetInt(360)));
	}

}
