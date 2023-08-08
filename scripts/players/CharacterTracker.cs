using Godot;
using System;
using System.Diagnostics;
using MechJamIV;

public partial class CharacterTracker : Node2D
{

    //[Export(PropertyHint.NodeType)]
    public PhysicsBody2D Target { get; private set; }

    [Export(PropertyHint.Layers2DPhysics)]
    public uint LineOfSightMask { get; private set; }

    [Export]
    public float LineOfSightDistance { get; private set; }

    [Export]
    public bool DrawReticle { get; set; }

    #region Node references

    private RayCast2D rayCast;
    private Sprite2D sprite2d;

    #endregion

    public override void _Ready()
    {
        rayCast = GetNode<RayCast2D>("RayCast2D");
        rayCast.CollisionMask = LineOfSightMask;

        sprite2d = GetNode<Sprite2D>("Sprite2D");
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
            if (DrawReticle)
            {
                sprite2d.GlobalPosition = Target.GlobalPosition;
                sprite2d.Rotate(0.5f * (float)delta);
            }

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

        return GlobalPosition.DirectionTo(Target.GlobalPosition);
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

    public void Track(PhysicsBody2D target)
    {
        Target = target;

        if (target != null)
        {
            sprite2d.Visible = DrawReticle;
            sprite2d.Rotation = 0.0f;

            //TODO we need another way to identify destructibles (we can't subscribe
            //     to signals through interfaces, AFAIK)
            // if (target is IDestructible d)
            // {
            //     d.Killed += () => Untrack(target);
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
        else
        {
            sprite2d.Visible = false;
        }
    }

    public void Track(PhysicsBody2D target, uint lineOfSightMask, float lineOfSightDistance)
    {
        LineOfSightMask = lineOfSightMask;
        LineOfSightDistance = lineOfSightDistance;

        rayCast.CollisionMask = lineOfSightMask;

        Track(target);
    }

    public void Untrack()
    {
        Target = null;

        sprite2d.Visible = false;
    }

}
