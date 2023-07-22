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
        rayCast = new RayCast2D();

        rayCast.Position = Vector2.Up; // offset so we don't collide with ground
        rayCast.CollideWithAreas = true;
        rayCast.CollideWithBodies = true;
        rayCast.CollisionMask = (uint)(CollisionLayerMask.World | CollisionLayerMask.Player);

        await this.AddChildDeferred(rayCast);
    }

    private void UpdateRayCastToTarget()
    {
        if (Target == null)
        {
            // this might help to hide the raycast the next time it is drawn
            rayCast.TargetPosition = Vector2.Zero;
        }
        else
        {
            rayCast.TargetPosition = GlobalTransform.Origin.DirectionTo(Target.GlobalTransform.Origin) * 1000.0f;
        }
    }

    public override void _Draw()
    {
        //DrawDashedLine(Vector2.Zero, Player.GlobalTransform.Origin - GlobalTransform.Origin, Colors.SkyBlue);
        DrawDashedLine(rayCast.Position, rayCast.GetCollisionPoint() - GlobalTransform.Origin, Colors.SkyBlue);
    }

}

#endif