using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MechJamIV;

public partial class Missile : Grenade
	,ITracker
{

	[Export]
	public float ThrustForce { get; set; } = 5_000.0f;

	[Export]
	public float TurnSpeed { get; set; } = 3_000f;

	#region Node references

	private GpuParticles2D gpuParticles2D;
	private CharacterAnimator characterAnimator;

	#endregion

	public override void _Ready()
	{
		base._Ready();

		gpuParticles2D = GetNode<GpuParticles2D>("GPUParticles2D");
		characterAnimator = GetNode<CharacterAnimator>("CharacterAnimator");

		BodyEntered += (body) => Hurt(Health, GlobalTransform.Origin, Vector2.Zero);

#if DEBUG
		AddRayCast();
#endif
	}

	public override void _Process(double delta)
	{
		if (Health <= 0)
		{
			return;
		}

#if DEBUG
		QueueRedraw();
#endif
	}

    public override void _PhysicsProcess(double delta)
    {
		if (Health <= 0)
		{
			return;
		}

		if (Target != null)
		{
			Vector2 directionToTarget = this.GetDirectionToTarget();

			float angleDiff = Mathf.RadToDeg(characterAnimator.SpriteFaceDirection.Rotated(Rotation).AngleTo(directionToTarget));
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

		ApplyForce(characterAnimator.SpriteFaceDirection.Rotated(Rotation) * ThrustForce * (float)delta);

#if DEBUG
		UpdateRayCastToTarget();
#endif
    }

	protected override void AnimateDeath()
	{
		base.AnimateDeath();

		// allow emitted particles to decay
		gpuParticles2D.Emitting = false;
	}

	#region ITracker

	public CollisionLayerMask LineOfSightMask { get; private set; }

	public float LineOfSightDistance { get; private set; } = 10_000.0f;

	public CharacterBase Target { get; private set; }

	public void Track(CharacterBase c, CollisionLayerMask lineOfSightMask)
	{
		Target = c;
		LineOfSightMask = lineOfSightMask;

		if (c != null)
		{
			Target.Killed += () => Untrack(c);
			// just in case we miss the Killed signal
			Target.TreeExiting += () => Untrack(c);
		}
	}

	private void Untrack(CharacterBase c)
	{
		// make sure we are still tracking the object that fired this event
		if (Target == c)
		{
			Target = null;
		}
	}

	#endregion

}
