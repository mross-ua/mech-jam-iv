using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class Missile : Grenade
{

	[Export]
	public Vector2 FaceDirection { get; set; } = Vector2.Up;

	[Export]
	public float ThrustForce { get; set; } = 1_000.0f;
	[Export]
	public float TurnSpeed { get; set; } = 3.0f;

	#region Node references

	protected Player Player { get; private set; }

	private GpuParticles2D gpuParticles2D;

	#endregion

	public override void _Ready()
	{
		base._Ready();

		Player = (Player)GetTree().GetFirstNodeInGroup("player");

		gpuParticles2D = GetNode<GpuParticles2D>("GPUParticles2D");

#if DEBUG
		AddRayCastToPlayer();
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

	private Vector2 GetMovementDirection(double delta)
	{
		Vector2 directionToPlayer = GetDirectionToPlayer();

		float angleDiff = FaceDirection.AngleTo(directionToPlayer);
		int turnDirection = Mathf.Sign(angleDiff);

		if (Mathf.Abs(angleDiff) < Mathf.DegToRad(TurnSpeed) * delta)
		{
			return directionToPlayer;
		}
		else
		{
			return FaceDirection + new Vector2(0.0f, Mathf.DegToRad(TurnSpeed) * (float)delta * turnDirection);
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
		UpdateRayCastToPlayer();
#endif
    }

	protected override void AnimateDeath()
	{
		base.AnimateDeath();

		gpuParticles2D.Visible = false;
	}

	protected Vector2 GetDirectionToPlayer()
	{
		return GlobalTransform.Origin.DirectionTo(Player.GlobalTransform.Origin);
	}

	// protected bool IsPlayerInFieldOfView()
	// {
	// 	return Mathf.RadToDeg(FaceDirection.AngleTo(GetDirectionToPlayer())) < FieldOfView;
	// }

	protected bool IsPlayerInLineOfSight()
	{
		Godot.Collections.Dictionary collision = GetWorld2D().DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters2D()
		{
			//TODO do we need to use Y basis rather than Vector2.Up?
			From = GlobalTransform.Origin + Vector2.Up, // offset so we don't collide with ground
			To = Player.GlobalTransform.Origin,
			Exclude = null,
			CollideWithBodies = true,
			CollideWithAreas = true,
			CollisionMask = (uint)(CollisionLayerMask.World | CollisionLayerMask.Player)
		});

		return collision.ContainsKey("collider") && collision["collider"].Obj == Player;
	}

}
