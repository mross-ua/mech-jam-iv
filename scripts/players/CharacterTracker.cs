using Godot;
using System;
using System.Diagnostics;
using MechJamIV;

public partial class CharacterTracker : Node2D
{

	//[Export(PropertyHint.NodeType)]
	public CollisionObject2D Target { get; private set; }

	[Export(PropertyHint.Layers2DPhysics)]
	public uint LineOfSightMask { get; private set; }

	[Export]
	public float LineOfSightDistance { get; private set; }

    #region Node references

    RayCast2D rayCast;

    #endregion

    public override void _Ready()
    {
		rayCast = GetNode<RayCast2D>("RayCast2D");
		rayCast.CollisionMask = LineOfSightMask;
    }

	public override void _Process(double delta)
	{
		if (OS.IsDebugBuild())
		{
			QueueRedraw();
		}
	}

    public override void _PhysicsProcess(double delta)
    {
		if (Target != null)
		{
			rayCast.TargetPosition = GetDirectionToTarget() * LineOfSightDistance;

			//TODO is this a good solution or a hack?
			// reverse rotation for parents/owners that have rotation enabled
            rayCast.Rotation = -GlobalTransform.Rotation;
		}
    }

    public override void _Draw()
    {
		if (OS.IsDebugBuild() && Target != null)
		{
			if (IsTargetInLineOfSight())
			{
				DrawLine(rayCast.Position, ToLocal(rayCast.GetCollisionPoint()), Colors.SkyBlue);
			}
			//TODO don't draw a line if the target is no longer in range
			//     (this will show a line if there is a wall in range but not the player)
			else if (rayCast.IsColliding())
			{
				DrawDashedLine(rayCast.Position, ToLocal(rayCast.GetCollisionPoint()), Colors.SkyBlue);
			}
		}
    }

	public Vector2 GetDirectionToTarget()
	{
		Debug.Assert(Target != null, "A target is not currently being tracked.");

		return GlobalTransform.Origin.DirectionTo(Target.GlobalTransform.Origin);
	}

	public bool IsTargetInFieldOfView(Vector2 faceDirection, float fieldOfView)
	{
		return Mathf.RadToDeg(faceDirection.AngleTo(GetDirectionToTarget())) < fieldOfView;
	}

	public bool IsTargetInLineOfSight()
	{
		Debug.Assert(Target != null, "A target is not currently being tracked.");

		return rayCast.GetCollider() == Target;
	}

	public void Track(CollisionObject2D target)
	{
		Target = target;

		if (target != null)
		{
			//TODO we need another way to identify destructibles (we can't subscribe
			//     to signals through interfaces, AFAIK)
			// if (target is IDestructible d)
			// {
			// 	d.Killed += () => Untrack(target);
			// }
			if (target is CharacterBase c)
			{
				c.Killed += () =>
				{
					// make sure we are still tracking the object that fired this event
					if (Target == target)
					{
						Untrack();
					}
				};
			}

			// just in case we miss the Killed signal
			target.TreeExiting += () =>
			{
				// make sure we are still tracking the object that fired this event
				if (Target == target)
				{
					Untrack();
				}
			};
		}
	}

	public void Track(CollisionObject2D target, uint lineOfSightMask, float lineOfSightDistance)
	{
		LineOfSightMask = lineOfSightMask;
		LineOfSightDistance = lineOfSightDistance;

		rayCast.CollisionMask = lineOfSightMask;

		Track(target);
	}

	public void Untrack()
	{
		Target = null;
	}

}
