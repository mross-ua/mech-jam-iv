using Godot;
using System;
using MechJamIV;

public partial class EnemyTroid : EnemyBase
{

    protected override Vector2 Gravity { get; set; } = Vector2.Zero;

    private DateTime lastTimePlayerSeen = DateTime.MinValue;

    protected override Vector2 GetMovementDirection_Idle()
    {
        return Vector2.Zero;
    }

    protected override Vector2 GetMovementDirection_Chase()
    {
        if (CharacterTracker.Target == null)
        {
            return Vector2.Zero;
        }

        return CharacterTracker.GetDirectionToTarget();
    }

    protected override Vector2 GetMovementDirection_Attacking()
    {
        // stay with player
        return GetMovementDirection_Chase();
    }

    protected override bool _IsJumping() => false;

    protected override void ProcessAction_Idle()
    {
        if (CharacterTracker.Target == null)
        {
            return;
        }
        else if (CharacterTracker.IsTargetInLineOfSight())
        {
            State = EnemyState.Chase;

            lastTimePlayerSeen = DateTime.Now;
        }
    }

    protected override void ProcessAction_Chase()
    {
        if (CharacterTracker.Target == null)
        {
            State = EnemyState.Idle;
        }
        else if (CharacterTracker.IsTargetInLineOfSight())
        {
            State = EnemyState.Attacking;

            lastTimePlayerSeen = DateTime.Now;
        }
        else if ((DateTime.Now - lastTimePlayerSeen).Seconds >= ChaseDuration)
        {
            State = EnemyState.Idle;
        }
    }

    protected override void ProcessAction_Attacking()
    {
        // NOTE: We currently have collision checks on
        //       hitboxes so attacks happen automatically.

        ProcessAction_Chase();
    }

    protected override void AnimateInjury(int damage, Vector2 globalPos, Vector2 normal)
    {
        this.EmitParticlesOnce(PointDamageEffect.Instantiate<GpuParticles2D>(), globalPos);
    }

}
