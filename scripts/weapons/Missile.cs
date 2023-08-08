using Godot;
using System;
using MechJamIV;

public partial class Missile : ExplosiveProjectile
{

    [Export]
    public float ThrustForce { get; set; }

    [Export]
    public float TurnSpeed { get; set; }

    #region Node references

    private GpuParticles2D gpuParticles2D;

    #endregion

    public override void _Ready()
    {
        base._Ready();

        gpuParticles2D = GetNode<GpuParticles2D>("GPUParticles2D");

        BodyEntered += (body) =>
        {
            if (IsFusePrimed)
            {
                Hurt(Health, GlobalPosition, Vector2.Zero);
            }
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Health <= 0)
        {
            return;
        }

        if (IsFusePrimed)
        {
            if (CharacterTracker.Target != null)
            {
                Vector2 directionToTarget = CharacterTracker.GetDirectionToTarget();

                float angleDiff = Mathf.RadToDeg(CharacterAnimator.SpriteFaceDirection.Rotated(Rotation).AngleTo(directionToTarget));
                int turnDirection = Mathf.Sign(angleDiff);

                float rotation = TurnSpeed * (float)delta;

                if (Mathf.Abs(angleDiff) < rotation)
                {
                    Rotate(Mathf.DegToRad(angleDiff));
                }
                else
                {
                    Rotate(Mathf.DegToRad(rotation) * turnDirection);
                }
            }

            ApplyForce(CharacterAnimator.SpriteFaceDirection.Rotated(Rotation) * ThrustForce * (float)delta);
        }
    }

    protected override void AnimateDeath()
    {
        base.AnimateDeath();

        // allow emitted particles to decay
        gpuParticles2D.Emitting = false;
    }

    #region IDetonable

    public override void PrimeFuse()
    {
        gpuParticles2D.Visible = true;
        GravityScale = 0.0f;

        base.PrimeFuse();
    }

    #endregion

}
