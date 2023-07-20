using Godot;
using System;
using MechJamIV;

public partial class EnemyTroid : EnemyBase
{

    protected override Vector2 Gravity { get; set; } = Vector2.Zero;

	private int chaseDuration = 1;
	private DateTime lastTimePlayerSeen = DateTime.MinValue;

	#region Resources

	private PackedScene acidSplatter = ResourceLoader.Load<PackedScene>("res://scenes/effects/acid_splatter.tscn");

	#endregion

	protected override Vector2 GetMovementDirection_Idle()
	{
        return Vector2.Zero;
	}

	protected override Vector2 GetMovementDirection_Chase()
	{
		return GetDirectionToPlayer();
	}

	protected override Vector2 GetMovementDirection_Attacking()
	{
        // stay with player
        return GetMovementDirection_Chase();
	}

    protected override bool IsJumping() => false;

	protected override void ProcessAction_Idle()
	{
        if (IsPlayerInLineOfSight())
        {
            State = EnemyState.Chase;

			lastTimePlayerSeen = DateTime.Now;
        }
	}

	protected override void ProcessAction_Chase()
	{
		if (IsPlayerInLineOfSight())
		{
			State = EnemyState.Attacking;

			lastTimePlayerSeen = DateTime.Now;
		}
		else if ((DateTime.Now - lastTimePlayerSeen).Seconds >= chaseDuration)
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

	protected override async void AnimateInjury(int damage, Vector2 position, Vector2 normal)
    {
        GpuParticles2D splatter = acidSplatter.Instantiate<GpuParticles2D>();
        splatter.GlobalPosition = position;
		splatter.Emitting = true;

		await GetTree().CurrentScene.AddChildDeferred(splatter);

		splatter.TimedFree(splatter.Lifetime + splatter.Lifetime * splatter.Randomness, processInPhysics:true);
    }

}
