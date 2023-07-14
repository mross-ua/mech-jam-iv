using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class ExplosiveBarrel : Barrel
{

	[Signal]
	public delegate void KilledEventHandler();

	[Export]
	public virtual int Health { get; set; } = 10;
	[Export]
	public int Damage { get; set; } = 80;
	[Export]
	public float ExplosionIntensity { get; set; } = 10_000.0f;

	private Godot.Collections.Array<Rid> bodiesToExclude;

	#region Node references

	private CharacterAnimator characterAnimator;
	private Area2D explosionAreaOfEffect;
	private CollisionShape2D collisionShape2D;

    #endregion

    public override void _Ready()
    {
		bodiesToExclude = new Godot.Collections.Array<Rid>(GetRid().Yield());

		characterAnimator = GetNode<CharacterAnimator>("CharacterAnimator");
		explosionAreaOfEffect = GetNode<Area2D>("ExplosionAreaOfEffect");
		collisionShape2D = GetNode<CollisionShape2D>("ExplosionAreaOfEffect/CollisionShape2D");
    }

	public void SetBodiesToExclude(IEnumerable<Rid> rids)
	{
		bodiesToExclude = new Godot.Collections.Array<Rid>(rids);
	}

	protected virtual void AnimateDeath() => characterAnimator.AnimateDeath();

	public override void Hurt(int damage, Vector2 position, Vector2 normal)
	{
		if (Health <= 0)
		{
			return;
		}

		base.Hurt(damage, position, normal);

		Health = Math.Max(0, Health - damage);

		EmitSignal(SignalName.Injured, damage);

		if (Health <= 0)
		{
			AnimateDeath();

			Explode();

			EmitSignal(SignalName.Killed);

			this.TimedFree(5.0f, processInPhysics:true);
		}
	}

	private void Explode()
	{
		PhysicsShapeQueryParameters2D queryParams = new ();
		queryParams.Transform = GlobalTransform;
		queryParams.Shape = collisionShape2D.Shape;
		queryParams.CollisionMask = explosionAreaOfEffect.CollisionMask;
		queryParams.Exclude = bodiesToExclude;

		foreach (Godot.Collections.Dictionary collision in GetWorld2D().DirectSpaceState.IntersectShape(queryParams))
		{
			// NOTE: We want to scale the damage and push force depending on the entity's
			//       distance from the explosion.

			// we assume the shape is a circle
			float radius = collisionShape2D.Shape.GetRect().Size.X / 2;

			if (collision["collider"].Obj is CharacterBase character)
			{
				Vector2 directionToCharacter = character.GlobalTransform.Origin - GlobalTransform.Origin;

				character.Hurt(Mathf.RoundToInt(Damage * radius / directionToCharacter.Length()), character.GlobalTransform.Origin, -directionToCharacter.Normalized());
				character.Velocity += ExplosionIntensity * directionToCharacter / directionToCharacter.LengthSquared();
			}
			else if (collision["collider"].Obj is Barrel barrel)
			{
				Vector2 directionToBarrel = barrel.GlobalTransform.Origin - GlobalTransform.Origin;

				barrel.Hurt(Mathf.RoundToInt(Damage * radius / directionToBarrel.Length()), barrel.GlobalTransform.Origin, -directionToBarrel.Normalized());
				barrel.ApplyImpulse(ExplosionIntensity * directionToBarrel / directionToBarrel.LengthSquared());
			}
		}
	}

}
