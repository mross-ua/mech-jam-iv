using Godot;
using System;
using MechJamIV;

public partial class CyberSteel : Objective
{

    #region Node references

    private AnimatedSprite2D animatedSprite2D;
    private CollisionShape2D collisionShape2D;

    #endregion

    public override void _Ready()
    {
        base._Ready();

        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        // make it flicker
        animatedSprite2D.AnimationLooped += () => animatedSprite2D.Rotate((float)GD.RandRange(0.0f, 2 * Mathf.Pi));

        collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");

        BodyEntered += (body) =>
        {
            Visible = false;
            collisionShape2D.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        };
    }

}
