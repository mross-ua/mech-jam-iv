using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class ProjectileEmitter : Node2D
	,IWeapon
{

	private int ammo = 0;

	[Export(PropertyHint.Layers2DPhysics)]
	public uint LineOfSightMask { get; set; }
	[Export]
	public PackedScene ProjectileBaseItem { get; set; }
	[Export]
	public float RoundsPerSecond { get; set; }
	[Export]
	public int Ammo
	{
		get
		{
			return ammo;
		}
		set
		{
			if (ammo != value)
			{
				ammo = value;

				EmitSignal(SignalName.Fired, ammo);
			}
		}
	}
	[Export]
	public float ImpulseStrength { get; set; }

	private bool isCoolingDown = false;

	private IList<PhysicsBody2D> bodiesToExclude = null;

	public void SetBodiesToExclude(IEnumerable<PhysicsBody2D> bodies)
	{
		bodiesToExclude = new List<PhysicsBody2D>(bodies);
	}

	public async void Fire(Vector2 globalPos, CharacterBase target)
	{
		if (isCoolingDown)
		{
			return;
		}
		else if (Ammo <= 0)
		{
			return;
		}

		isCoolingDown = true;

		Ammo--;

		ProjectileBase projectile = ProjectileBaseItem.Instantiate<ProjectileBase>();
		projectile.GlobalTransform = GlobalTransform;

		projectile.SetBodiesToExclude(bodiesToExclude);

		await GetTree().CurrentScene.AddChildDeferred(projectile);

		projectile.ApplyImpulse((globalPos - GlobalTransform.Origin).Normalized() * ImpulseStrength);

		if (projectile is IDetonable d)
		{
			d.PrimeFuse();
		}

		if (projectile is ITracker t)
		{
			t.Track(target, (CollisionLayerMask)LineOfSightMask);
		}

		EmitSignal(SignalName.Fired, ammo);

		if (!Mathf.IsZeroApprox(RoundsPerSecond))
		{
			await ToSignal(GetTree().CreateTimer(1.0f / RoundsPerSecond), SceneTreeTimer.SignalName.Timeout);
		}

		isCoolingDown = false;
	}

	#region IWeapon

	[Signal]
	public delegate void FiredEventHandler(int ammoRemaining);

	#endregion

}
