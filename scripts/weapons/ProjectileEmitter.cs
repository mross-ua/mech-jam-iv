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
		if (bodies == null)
		{
			bodiesToExclude = null;
		}
		else
		{
			bodiesToExclude = new List<CollisionObject2D>(bodies);
		}
	}

	protected async override void _Fire(Vector2 globalPos, CollisionObject2D target = null)
	{
		ProjectileBase projectile = ProjectileBaseItem.Instantiate<ProjectileBase>();
		projectile.GlobalTransform = GlobalTransform;

		projectile.SetBodiesToExclude(bodiesToExclude);

		await GetTree().CurrentScene.AddChildDeferred(projectile);

		Vector2 dir = (globalPos - GlobalPosition).Normalized();

		projectile.ApplyImpulse(dir * ImpulseStrength);

		if (projectile is IDetonable d)
		{
			d.PrimeFuse();
		}

		//TODO we need another way to identify trackers (since we got rid of ITracker)
		if (projectile is Missile m)
		{
			if (target == null)
			{
				m.Rotate(m.CharacterAnimator.SpriteFaceDirection.AngleTo(dir));
			}

			m.CharacterTracker.Track(target);
		}
	}

	#region IWeapon

	public override PickupType WeaponType
	{
		get
		{
			//TODO this is highly inefficient

			ProjectileBase item = ProjectileBaseItem.Instantiate<ProjectileBase>();

			PickupType pickupType = item.WeaponType;

			// we have to free it because we don't add it to the scene tree
			item.Free();

			return pickupType;
		}
	}

    public override Texture2D SpriteTexture
	{
		get
		{
			//TODO this is highly inefficient

			ProjectileBase item = ProjectileBaseItem.Instantiate<ProjectileBase>();

			Texture2D texture2d = item.SpriteTexture;

			// we have to free it because we don't add it to the scene tree
			item.Free();

			return texture2d;
		}
	}

	#endregion

}
