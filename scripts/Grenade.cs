using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class Grenade : ExplosiveBarrel,
	IProjectile
{

	public override int Health { get; set; } = 1;

	#region Node references

	private Timer fuseTimer;

	#endregion

    public override void _Ready()
    {
        base._Ready();

		fuseTimer = GetNode<Timer>("FuseTimer");
		fuseTimer.Timeout += () => Hurt(Health, GlobalTransform.Origin, Vector2.Zero);
    }

	public override void Prime()
	{
		fuseTimer.Start();
	}

}
