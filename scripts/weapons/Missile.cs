using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MechJamIV;

public partial class Missile : ExplosiveProjectile
{

	[Export]
	public float ThrustForce { get; set; } = 5_000.0f;

	[Export]
	public float TurnSpeed { get; set; } = 3_000f;

	#region Node references

	public CharacterTracker CharacterTracker { get; private set; }

	private GpuParticles2D GpuParticles2D;

	#endregion

	public override void _Ready()
	{
		base._Ready();

		GpuParticles2D = GetNode<GpuParticles2D>("GPUParticles2D");

		CharacterTracker = GetNodeOrNull<CharacterTracker>("CharacterTracker");

		BodyEntered += (body) =>
		{
			if (IsFusePrimed)
			{
				Hurt(Health, GlobalPosition, Vector2.Zero);
			}
		};
	}

    public override void _PhysicsProcess(double delta)
    {
		if (Health <= 0)
		{
			return;
		}

		if (IsFusePrimed)
		{
			if (CharacterTracker.Target != null)
			{
				Vector2 directionToTarget = CharacterTracker.GetDirectionToTarget();

				float angleDiff = Mathf.RadToDeg(CharacterAnimator.SpriteFaceDirection.Rotated(Rotation).AngleTo(directionToTarget));
				int turnDirection = Mathf.Sign(angleDiff);

				float rotation = TurnSpeed * (float)delta;

				if (Mathf.Abs(angleDiff) < rotation)
				{
					Rotate(Mathf.DegToRad(angleDiff));
				}
				else
				{
					Rotate(Mathf.DegToRad(rotation) * turnDirection);
				}
			}

			ApplyForce(CharacterAnimator.SpriteFaceDirection.Rotated(Rotation) * ThrustForce * (float)delta);
		}
    }

	protected override void AnimateDeath()
	{
		base.AnimateDeath();

		// allow emitted particles to decay
		GpuParticles2D.Emitting = false;
	}

	#region IDestructible

	public override void Hurt(int damage, Vector2 globalPos, Vector2 normal)
	{
		if (Health <= 0)
		{
			return;
		}

		base.Hurt(damage, globalPos, normal);

		if (Health <= 0)
		{
			CharacterTracker.Untrack();
		}
	}

    #endregion

    #region IDetonable

    public override void PrimeFuse()
    {
		GpuParticles2D.Visible = true;
		GravityScale = 0.0f;

        base.PrimeFuse();
    }

	#endregion

}
