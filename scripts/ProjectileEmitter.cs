using Godot;
using System;
using MechJamIV;

public partial class ProjectileEmitter : Node2D
{

	[Export]
	public PackedScene ProjectileBaseItem { get; set; }
	[Export]
	public int Ammo { get; set; } = 0;
	[Export]
	public float ImpulseStrength { get; set; } = 500.0f;

	public async void Fire(Vector2 globalPos)
	{
		if (Ammo <= 0)
		{
			return;
		}

		Ammo--;

		ProjectileBase projectile = ProjectileBaseItem.Instantiate<ProjectileBase>();
		projectile.GlobalTransform = GlobalTransform;

		await GetTree().CurrentScene.AddChildDeferred(projectile);

		projectile.Prime();

		projectile.ApplyImpulse((globalPos - GlobalTransform.Origin).Normalized() * ImpulseStrength);
	}

}
