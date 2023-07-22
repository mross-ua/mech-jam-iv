using Godot;
using System;
using MechJamIV;

public partial class ProjectileEmitter : Node2D
{

	[Export(PropertyHint.Layers2DPhysics)]
	public uint LineOfSightMask { get; set; }
	[Export]
	public PackedScene ProjectileBaseItem { get; set; }
	[Export]
	public int Ammo { get; set; } = 0;
	[Export]
	public float ImpulseStrength { get; set; } = 500.0f;

	public async void Fire(Vector2 globalPos, CharacterBase target)
	{
		if (Ammo <= 0)
		{
			return;
		}

		Ammo--;

		ProjectileBase projectile = ProjectileBaseItem.Instantiate<ProjectileBase>();
		projectile.GlobalTransform = GlobalTransform;

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
	}

}
