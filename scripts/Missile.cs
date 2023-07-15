using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class Missile : Grenade
{

	[Export]
	public float Thrust { get; set; } = 3.0f;
	[Export]
	public float RotationSpeed { get; set; } = 3.0f;

	#region Node references

	protected Player Player { get; private set; }

	#endregion

	public override void _Ready()
	{
		base._Ready();

		Player = (Player)GetTree().GetFirstNodeInGroup("player");

#if DEBUG
		AddRayCastToPlayer();
#endif
	}

    public override void _PhysicsProcess(double delta)
    {
		if (Health <= 0)
		{
			return;
		}

		ApplyImpulse(Vector2.Up * 3);

#if DEBUG
		UpdateRayCastToPlayer();
#endif
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
