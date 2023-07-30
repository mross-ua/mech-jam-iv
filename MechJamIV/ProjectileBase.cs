using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MechJamIV;

namespace MechJamIV {
	public abstract partial class ProjectileBase : RigidBody2D
		,ICollidable
	{

		[Export]
		public PackedScene PointDamageEffect { get; set; }

    	public abstract Texture2D SpriteTexture { get; }

		public void SetBodiesToExclude(IEnumerable<CollisionObject2D> bodies)
		{
			//TODO remove previously excluded bodies?

			foreach (CollisionObject2D body in bodies)
			{
				AddCollisionExceptionWith(body);
			}
		}

		#region ICollidable

		[Signal]
		public delegate void InjuredEventHandler(int damage);

		public abstract void Hurt(int damage, Vector2 globalPos, Vector2 normal);

		#endregion

	}
}
