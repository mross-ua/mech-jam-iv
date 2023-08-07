using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MechJamIV {
	public partial class ExplosiveProjectile : Projectile
		,IDestructible
		,IDetonable
	{

		[Export]
		public int ExplosionDamage { get; set; } = 80;

		[Export]
		public float ExplosionIntensity { get; set; } = 10_000.0f;

		#region Node references

		public CharacterAnimator CharacterAnimator;

		private CollisionShape2D collisionShape2D;
		private Area2D explosionAreaOfEffect;
		private CollisionShape2D explosionCollisionShape2D;

		#endregion

		public override void _Ready()
		{
			base._Ready();

			CharacterAnimator = GetNode<CharacterAnimator>("CharacterAnimator");

			collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");

			explosionAreaOfEffect = GetNode<Area2D>("ExplosionAreaOfEffect");
			explosionCollisionShape2D = GetNode<CollisionShape2D>("ExplosionAreaOfEffect/CollisionShape2D");
		}

        protected override bool CanBePickedUp() => !IsFusePrimed;

		protected virtual void AnimateDeath() => CharacterAnimator.AnimateDeath();

		#region IDestructible

		[Signal]
		public delegate void KilledEventHandler();

		[Signal]
		public delegate void HealedEventHandler(int health);

		[Export]
		public int Health { get; set; } = 10;

		public override void Hurt(int damage, Vector2 globalPos, Vector2 normal)
		{
			if (Health <= 0)
			{
				return;
			}

			base.Hurt(damage, globalPos, normal);

			Health = Math.Max(0, Health - damage);

			EmitSignal(SignalName.Injured, damage);

			if (Health <= 0)
			{
				AnimateDeath();

				Detonate();

				EmitSignal(SignalName.Killed);

				// NOTE: We disable the collision shape and wait to
				//       free so the death animation can fully play.
				//       In order to support that, we have to freeze
				//       the projectile or else the death animation
				//       will continue moving/falling and it looks bad.

				SetDeferred(PropertyName.Freeze, true);
				collisionShape2D.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

				this.TimedFree(5.0f);
			}
		}

		public void Heal(int health)
		{
			// do nothing
		}

		#endregion

		#region IDetonable

		[Signal]
		public delegate void DetonatedEventHandler();

		[Export]
		public float FuseDelay { get; set; }

		protected bool IsFusePrimed { get; private set; } = false;

		public async virtual void PrimeFuse()
		{
			if (Health <= 0)
			{
				return;
			}
			else if (IsFusePrimed)
			{
				return;
			}

			IsFusePrimed = true;

			await ToSignal(GetTree().CreateTimer(FuseDelay, false, true), SceneTreeTimer.SignalName.Timeout);

			if (Health <= 0)
			{
				return;
			}

			EmitSignal(SignalName.Detonated);

			Hurt(Health, GlobalPosition, Vector2.Zero);
		}

		private void Detonate()
		{
			// we assume the shape is a circle
			float radius = explosionCollisionShape2D.Shape.GetRect().Size.X / 2;

			foreach (Node2D node in explosionAreaOfEffect.GetOverlappingBodies().Where(n => n != this))
			{
				// NOTE: We scale the damage and push force depending on
				//       the node's distance from the explosion.

				if (node is CharacterBase character)
				{
					Vector2 dir = character.GlobalPosition - GlobalPosition;

					character.Hurt(Mathf.RoundToInt(ExplosionDamage * radius / dir.LengthSquared()), character.GlobalPosition, -dir.Normalized());
					character.Velocity += ExplosionIntensity * dir / dir.LengthSquared();
				}
				else if (node is Projectile projectile)
				{
					Vector2 dir = projectile.GlobalPosition - GlobalPosition;

					projectile.Hurt(Mathf.RoundToInt(ExplosionDamage * radius / dir.LengthSquared()), projectile.GlobalPosition, -dir.Normalized());
					projectile.ApplyImpulse(ExplosionIntensity * dir / dir.LengthSquared());
				}
			}
		}

		#endregion

	}
}
