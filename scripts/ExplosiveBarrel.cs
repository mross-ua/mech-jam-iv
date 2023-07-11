using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class ExplosiveBarrel : Barrel
{

	[Export]
	public virtual int Health { get; set; } = 10;
	[Export]
	public int Damage { get; set; } = 25;
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

	public override async void HurtAsync(int damage, Vector2 normal)
	{
		if (Health <= 0)
		{
			return;
		}

		base.HurtAsync(damage, normal);

		Health = Math.Max(0, Health - damage);

		EmitSignal(SignalName.Hurt, damage);

		if (Health <= 0)
		{
			AnimateDeath();

			Explode();

			await ToSignal(GetTree().CreateTimer(5.0f), SceneTreeTimer.SignalName.Timeout);

			QueueFree();
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
			if (collision["collider"].Obj is CharacterBase character)
			{
				Vector2 directionToCharacter = character.GlobalTransform.Origin - GlobalTransform.Origin;

				character.HurtAsync(Damage, directionToCharacter.Normalized());
				character.Velocity += ExplosionIntensity * directionToCharacter / directionToCharacter.LengthSquared();
			}
			else if (collision["collider"].Obj is Barrel barrel)
			{
				Vector2 directionToBarrel = barrel.GlobalTransform.Origin - GlobalTransform.Origin;

				barrel.HurtAsync(Damage, directionToBarrel.Normalized());
				barrel.ApplyImpulse(ExplosionIntensity * directionToBarrel / directionToBarrel.LengthSquared());
			}
		}
	}

}
