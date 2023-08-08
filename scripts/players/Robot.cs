using Godot;
using System;
using MechJamIV;

public partial class Robot : CharacterBase
{

    protected override Vector2 Gravity { get; set; } = Vector2.Zero;

    protected override Vector2 GetMovementDirection()
    {
        if (CharacterTracker.Target == null)
        {
            return Vector2.Zero;
        }

        //TODO there's a hard cast here--need to refactor...something.
        //     we don't need to rely on the marker--the robot will eventually
        //     have other logic or user input
        return GlobalPosition.DirectionTo(((Player)CharacterTracker.Target).RobotMarker.GlobalPosition);
    }

    protected override bool _IsJumping() => false;

    protected override void ProcessAction()
    {
        //TODO
    }

    protected override void AnimateInjury(int damage, Vector2 position, Vector2 normal)
    {
        //TODO
    }

    #region ICollidable

    public override void Hurt(int damage, Vector2 globalPos, Vector2 normal)
    {
        // ignore damage
        //base.Hurt(damage, globalPos, normal);
    }

    #endregion

}
