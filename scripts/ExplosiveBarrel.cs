using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MechJamIV;

public partial class ExplosiveBarrel : Barrel
	,IDestructible
	,IDetonable
{

	[Export]
	public int ExplosionDamage { get; set; } = 80;
	[Export]
	public float ExplosionIntensity { get; set; } = 10_000.0f;

	private Godot.Collections.Array<Rid> bodiesToExclude;

	#region Node references

	protected CharacterAnimator CharacterAnimator;

	private CollisionShape2D collisionShape2D;
	private Area2D explosionAreaOfEffect;
	private CollisionShape2D explosionCollisionShape2D;

    #endregion

    public override void _Ready()
    {
		bodiesToExclude = new Godot.Collections.Array<Rid>(GetRid().Yield());

		CharacterAnimator = GetNode<CharacterAnimator>("CharacterAnimator");
		collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");

		explosionAreaOfEffect = GetNode<Area2D>("ExplosionAreaOfEffect");
		explosionCollisionShape2D = GetNode<CollisionShape2D>("ExplosionAreaOfEffect/CollisionShape2D");
    }

	public void SetBodiesToExclude(IEnumerable<Rid> resourceIds)
	{
		bodiesToExclude = new Godot.Collections.Array<Rid>(GetRid().Yield().Concat(resourceIds));
	}

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

			collisionShape2D.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

			this.TimedFree(5.0f, processInPhysics:true);
		}
	}

	public void Heal(int health)
	{
		// do nothing
	}

	#endregion

	#region IDetonatable

	[Signal]
	public delegate void DetonatedEventHandler();

	[Export]
	public float FuseDelay { get; set; } = 4.0f;

	private bool isFusePrimed = false;

	public async void PrimeFuse()
	{
		if (Health <= 0)
		{
			return;
		}
		else if (isFusePrimed)
		{
			return;
		}

		isFusePrimed = true;

		await ToSignal(GetTree().CreateTimer(FuseDelay, processInPhysics:true), SceneTreeTimer.SignalName.Timeout);

		if (Health <= 0)
		{
			return;
		}

        EmitSignal(SignalName.Detonated);

		Hurt(Health, GlobalTransform.Origin, Vector2.Zero);
	}

	private void Detonate()
	{
		PhysicsShapeQueryParameters2D queryParams = new ();
		queryParams.Transform = GlobalTransform;
		queryParams.Shape = explosionCollisionShape2D.Shape;
		queryParams.CollisionMask = explosionAreaOfEffect.CollisionMask;
		queryParams.Exclude = bodiesToExclude;

		foreach (Godot.Collections.Dictionary collision in GetWorld2D().DirectSpaceState.IntersectShape(queryParams))
		{
			// NOTE: We want to scale the damage and push force depending on the entity's
			//       distance from the explosion.

			// we assume the shape is a circle
			float radius = explosionCollisionShape2D.Shape.GetRect().Size.X / 2;

			if (collision["collider"].Obj is CharacterBase character)
			{
				Vector2 directionToCharacter = character.GlobalTransform.Origin - GlobalTransform.Origin;

				character.Hurt(Mathf.RoundToInt(ExplosionDamage * radius / directionToCharacter.Length()), character.GlobalTransform.Origin, -directionToCharacter.Normalized());
				character.Velocity += ExplosionIntensity * directionToCharacter / directionToCharacter.LengthSquared();
			}
			else if (collision["collider"].Obj is Barrel barrel)
			{
				Vector2 directionToBarrel = barrel.GlobalTransform.Origin - GlobalTransform.Origin;

				barrel.Hurt(Mathf.RoundToInt(ExplosionDamage * radius / directionToBarrel.Length()), barrel.GlobalTransform.Origin, -directionToBarrel.Normalized());
				barrel.ApplyImpulse(ExplosionIntensity * directionToBarrel / directionToBarrel.LengthSquared());
			}
		}
	}

	#endregion

}
