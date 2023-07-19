#if DEBUG

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MechJamIV {
    public abstract partial class EnemyBase : CharacterBase
    {

        #region Node references

        RayCast2D rayCast;

        #endregion

        private void AddRayCastToPlayer()
        {
            rayCast = new RayCast2D();

            rayCast.Position = Vector2.Up; // offset so we don't collide with ground
            rayCast.CollideWithAreas = true;
            rayCast.CollideWithBodies = true;
            rayCast.CollisionMask = (uint)(CollisionLayerMask.World | CollisionLayerMask.Player);

            UpdateRayCastToPlayer();

            AddChild(rayCast);
        }

        private void UpdateRayCastToPlayer()
        {
            rayCast.TargetPosition = GlobalTransform.Origin.DirectionTo(Player.GlobalTransform.Origin) * 1000.0f;
        }

        public override void _Draw()
        {
            base._Draw();

            //DrawDashedLine(Vector2.Zero, Player.GlobalTransform.Origin - GlobalTransform.Origin, Colors.SkyBlue);
            DrawDashedLine(rayCast.Position, rayCast.GetCollisionPoint() - GlobalTransform.Origin, Colors.SkyBlue);
        }

    }
}

#endif
