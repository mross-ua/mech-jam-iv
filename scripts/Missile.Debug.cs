#if DEBUG

using MechJamIV;

public partial class Missile : Grenade
{

    #region Node references

    RayCast2D rayCast;

    #endregion

    private async void AddRayCastToPlayer()
    {
        rayCast = new RayCast2D {
            Position = Vector2.Up, // offset so we don't collide with ground
            CollideWithAreas = true,
            CollideWithBodies = true,
            CollisionMask = (uint)(CollisionLayerMask.World | CollisionLayerMask.Player)
        };

        await this.AddChildDeferred(rayCast);

        UpdateRayCastToPlayer();
    }

    private void UpdateRayCastToPlayer() => 
        rayCast.TargetPosition = GlobalTransform.Origin.DirectionTo(Player.GlobalTransform.Origin) * 1000.0f;

    public override void _Draw() =>
        //DrawDashedLine(Vector2.Zero, Player.GlobalTransform.Origin - GlobalTransform.Origin, Colors.SkyBlue);
        DrawDashedLine(rayCast.Position, rayCast.GetCollisionPoint() - GlobalTransform.Origin, Colors.SkyBlue);

}

#endif