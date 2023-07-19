using Godot;
using System;
using MechJamIV;

public partial class EnemyMech : EnemyBase
{

    public override Vector2 FaceDirection { get; set; } = Vector2.Left;
	public override float MoveAcceleration { get; set; } = 5.0f;
	public override float MaxMoveSpeed { get; set; } = 30.0f;

	private int chaseDuration = 10;
	private DateTime lastTimePlayerSeen = DateTime.MinValue;

	#region Node references

	private Timer attackTimer;

	#endregion

	#region Resources

	private PackedScene missileResource = ResourceLoader.Load<PackedScene>("res://scenes/weapons/missile.tscn");
	private PackedScene shrapnelSplatter = ResourceLoader.Load<PackedScene>("res://scenes/effects/shrapnel_splatter.tscn");

	#endregion

	public override void _Ready()
	{
		base._Ready();

		attackTimer = GetNode<Timer>("AttackTimer");
	}

	protected override Vector2 GetMovementDirection_Idle()
	{
		if (RandomHelper.GetSingle() < 0.01f)
		{
			if (FaceDirection.IsEqualApprox(Vector2.Left))
			{
				return Vector2.Right;
			}
			else
			{
				return Vector2.Left;
			}
		}

		return Vector2.Zero;
	}

	protected override Vector2 GetMovementDirection_Chase()
	{
		return new Vector2(GetDirectionToPlayer().X, 0.0f).Normalized();
	}

	protected override Vector2 GetMovementDirection_Attacking()
	{
		return Vector2.Zero;
	}

    protected override bool IsJumping() => false;

	protected override void ProcessAction_Idle()
	{
		if (IsPlayerInFieldOfView() && IsPlayerInLineOfSight())
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
		if (!IsPlayerInLineOfSight())
		{
			State = EnemyState.Chase;

			return;
		}
		else if (attackTimer.TimeLeft > 0)
		{
			return;
		}

		Missile missile = missileResource.Instantiate<Missile>();
		GetTree().CurrentScene.AddChild(missile);

		missile.GlobalTransform = GlobalTransform;

		missile.Prime();

		attackTimer.Start();
	}

	protected override void AnimateInjury(int damage, Vector2 position, Vector2 normal)
    {
		GpuParticles2D splatter = shrapnelSplatter.Instantiate<GpuParticles2D>();

		GetTree().CurrentScene.AddChild(splatter);

		splatter.GlobalPosition = position;

		splatter.Emitting = true;

		splatter.TimedFree(splatter.Lifetime + splatter.Lifetime * splatter.Randomness, processInPhysics:true);
    }

}
