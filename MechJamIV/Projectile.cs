using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MechJamIV;

namespace MechJamIV {
	public partial class Projectile : RigidBody2D
		,ICollidable
	{

		[Export]
		public PackedScene PointDamageEffect { get; set; }

		[Export(PropertyHint.Enum)]
		public PickupType WeaponType { get; set; }

		[Export]
		public Texture2D UISprite { get; set; }

		public void SetBodiesToExclude(IEnumerable<CollisionObject2D> bodies)
		{
			//TODO remove previously excluded bodies?

			foreach (CollisionObject2D body in bodies)
			{
				AddCollisionExceptionWith(body);
			}
		}

		private void AnimateInjury(Vector2 globalPos)
		{
			this.EmitParticlesOnce(PointDamageEffect.Instantiate<GpuParticles2D>(), globalPos);
		}

		#region ICollidable

		[Signal]
		public delegate void InjuredEventHandler(int damage);

		public virtual void Hurt(int damage, Vector2 globalPos, Vector2 normal)
		{
			AnimateInjury(globalPos);
		}

		#endregion

	}
}
