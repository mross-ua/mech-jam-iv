using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class Grenade : ExplosiveBarrel,
	IProjectile
{

	public override int Health { get; set; } = 1;

	#region Node references

	private Timer explosionTimer;

	#endregion

    public override void _Ready()
    {
        base._Ready();

		explosionTimer = GetNode<Timer>("ExplosionTimer");
		explosionTimer.Timeout += () => Hurt(Health, GlobalTransform.Origin, Vector2.Zero);
    }

	public override void Prime()
	{
		explosionTimer.Start();
	}

}
