using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MechJamIV;

public partial class Missile : Grenade
	,ITracker
{

	[Export]
	public Vector2 FaceDirection { get; set; } = Vector2.Up;

	[Export]
	public float ThrustForce { get; set; } = 5_000.0f;
	[Export]
	public float TurnSpeed { get; set; } = 3_000f;

	#region Node references

	private GpuParticles2D gpuParticles2D;

	#endregion

	public override void _Ready()
	{
		base._Ready();

		gpuParticles2D = GetNode<GpuParticles2D>("GPUParticles2D");

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

		AnimateMovement();

#if DEBUG
		QueueRedraw();
#endif
	}

	private Vector2 GetMovementDirection(double delta)
	{
		if (Target == null)
		{
			return FaceDirection;
		}

		Vector2 directionToPlayer = this.GetDirectionToTarget();

		float angleDiff = Mathf.RadToDeg(FaceDirection.AngleTo(directionToPlayer));
		int turnDirection = Mathf.Sign(angleDiff);

		float rotation = TurnSpeed * (float)delta;

		if (Mathf.Abs(angleDiff) < rotation)
		{
			return directionToPlayer;
		}
		else
		{
			return FaceDirection.Rotated(Mathf.DegToRad(rotation) * turnDirection);
		}
	}

    public override void _PhysicsProcess(double delta)
    {
		if (Health <= 0)
		{
			return;
		}

		FaceDirection = GetMovementDirection(delta);

		ApplyForce(FaceDirection * ThrustForce * (float)delta);

#if DEBUG
		UpdateRayCastToTarget();
#endif
    }

	protected void AnimateMovement()
	{
		// NOTE: Rotating the graphics is a hack because we are using
		//       FaceDirection rather than a built-in property.

		CharacterAnimator.Rotation = Vector2.Up.AngleTo(FaceDirection);
		gpuParticles2D.Rotation = Vector2.Up.AngleTo(FaceDirection);
	}

	protected override void AnimateDeath()
	{
		base.AnimateDeath();

		gpuParticles2D.Emitting = false;
	}

	#region ITracker

	public CollisionLayerMask LineOfSightMask { get; private set; }

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
