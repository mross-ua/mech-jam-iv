using Godot;
using System;
using MechJamIV;

public partial class EnemyTroid : EnemyBase
{

    protected override Vector2 Gravity { get; set; } = Vector2.Zero;

    private DateTime lastTimePlayerSeen = DateTime.MinValue;

    protected override Vector2 GetMovementDirectionForIdleState()
    {
        return Vector2.Zero;
    }

    protected override Vector2 GetMovementDirectionForChaseState()
    {
        if (CharacterTracker.Target == null)
        {
            return Vector2.Zero;
        }

        return CharacterTracker.GetDirectionToTarget();
    }

    protected override Vector2 GetMovementDirectionForAttackState()
    {
        // stay with player
        return GetMovementDirectionForChaseState();
    }

    protected override bool IsJumping()
    {
        return false;
    }

    protected override void ProcessActionForIdleState()
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

    protected override void ProcessActionForChaseState()
    {
        if (CharacterTracker.Target == null)
        {
            State = EnemyState.Idle;
        }
        else if (CharacterTracker.IsTargetInLineOfSight())
        {
            State = EnemyState.Attack;

            lastTimePlayerSeen = DateTime.Now;
        }
        else if ((DateTime.Now - lastTimePlayerSeen).Seconds >= ChaseDuration)
        {
            State = EnemyState.Idle;
        }
    }

    protected override void ProcessActionForAttackState()
    {
        // NOTE: We currently have collision checks on
        //       hitboxes so attacks happen automatically.

        ProcessActionForChaseState();
    }

    protected override void AnimateInjury(int damage, Vector2 globalPos, Vector2 normal)
    {
        this.EmitParticlesOnce(PointDamageEffect.Instantiate<GpuParticles2D>(), globalPos);
    }

}
