using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class Grenade : ExplosiveBarrel
{

	[Export]
	public override int Health { get; set; } = 1;

	#region Node references

	private Timer explosionTimer;

	#endregion

    public override void _Ready()
    {
        base._Ready();

		explosionTimer = GetNode<Timer>("ExplosionTimer");
		explosionTimer.Timeout += () => Hurt(Health, Vector2.Zero);
    }

}
