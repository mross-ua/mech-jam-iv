#if DEBUG

using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class Missile : Grenade
    ,ITracker
{

    #region Node references

    RayCast2D rayCast;

    #endregion

    private async void AddRayCast()
    {
        rayCast = new ();

        rayCast.CollideWithAreas = true;
        rayCast.CollideWithBodies = true;

        await this.AddChildDeferred(rayCast);
    }

    private void UpdateRayCastToTarget()
    {
        if (Target == null)
        {
            // this might help to hide the raycast the next time it is drawn
            rayCast.TargetPosition = Vector2.Zero;
            rayCast.CollisionMask = 0;
        }
        else
        {
            rayCast.TargetPosition = this.GetDirectionToTarget() * LineOfSightDistance;
            rayCast.CollisionMask = (uint)LineOfSightMask;

            //TODO this is a hack because I couldn't figure out the
            //     operations to apply to rayCast.TargetPosition
            rayCast.Rotation = -Rotation;
        }
    }

    public override void _Draw()
    {
        if (rayCast.IsColliding())
        {
            DrawDashedLine(rayCast.Position, ToLocal(rayCast.GetCollisionPoint()), Colors.SkyBlue);
        }
    }

}

#endif