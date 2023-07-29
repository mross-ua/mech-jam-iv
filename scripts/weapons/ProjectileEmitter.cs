using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class ProjectileEmitter : WeaponBase
{

	[Export]
	public PackedScene ProjectileBaseItem { get; set; }

	[Export]
	public float ImpulseStrength { get; set; }

	private IList<CollisionObject2D> bodiesToExclude = null;

	public override void SetBodiesToExclude(IEnumerable<CollisionObject2D> bodies)
	{
		bodiesToExclude = new List<CollisionObject2D>(bodies);
	}

	protected async override void _Fire(Vector2 globalPos, CollisionObject2D target = null)
	{
		ProjectileBase projectile = ProjectileBaseItem.Instantiate<ProjectileBase>();
		projectile.GlobalTransform = GlobalTransform;

		projectile.SetBodiesToExclude(bodiesToExclude);

		await GetTree().CurrentScene.AddChildDeferred(projectile);

		Vector2 dir = (globalPos - GlobalTransform.Origin).Normalized();

		projectile.ApplyImpulse(dir * ImpulseStrength);

		if (projectile is IDetonable d)
		{
			d.PrimeFuse();
		}

		//TODO we need another way to identify trackers (since we got rid of ITracker)
		if (projectile is Missile m)
		{
			//TODO this is needed only if there is no target
			m.Rotate(m.CharacterAnimator.SpriteFaceDirection.AngleTo(dir));

			m.CharacterTracker.Track(target);
		}
	}

}
