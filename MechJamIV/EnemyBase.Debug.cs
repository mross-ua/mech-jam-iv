#if DEBUG

using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MechJamIV {
    public abstract partial class EnemyBase : CharacterBase
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
            }
        }

        public override void _Draw()
        {
            base._Draw();

            if (rayCast.IsColliding())
            {
                DrawDashedLine(rayCast.Position, ToLocal(rayCast.GetCollisionPoint()), Colors.SkyBlue);
            }
        }

    }
}

#endif
