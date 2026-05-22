using Godot;
using System;
using MechJamIV.Base;
using MechJamIV.Enums;
using System.Diagnostics;

namespace MechJamIV;

public partial class EnemyTroid : EnemyBase
{

    protected override Vector2 Gravity { get; set; } = Vector2.Zero;

    private DateTime lastTimePlayerSeen = DateTime.MinValue;

    public override void _Ready()
    {
        base._Ready();

        Debug.Assert(CharacterTracker is not null, $"{nameof(CharacterTracker)} must not be null");
    }

    protected override Vector2 GetMovementDirectionForIdleState()
    {
        return Vector2.Zero;
    }

    protected override Vector2 GetMovementDirectionForChaseState()
    {
        if (CharacterTracker!.Target is null)
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
        if (CharacterTracker!.Target is null)
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
        if (CharacterTracker!.Target is null)
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

}
