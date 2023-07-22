using Godot;
using System;
using MechJamIV;

public partial class EnemyMech : EnemyBase
{

	private int chaseDuration = 10;
	private DateTime lastTimePlayerSeen = DateTime.MinValue;

	#region Node references

	private Timer attackTimer;

	#endregion

	#region Resources

	private static readonly PackedScene missileResource = ResourceLoader.Load<PackedScene>("res://scenes/weapons/missile.tscn");
	private static readonly PackedScene shrapnelSplatter = ResourceLoader.Load<PackedScene>("res://scenes/effects/shrapnel_splatter.tscn");

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

		return FaceDirection;
	}

	protected override Vector2 GetMovementDirection_Chase()
	{
		if (Target == null)
		{
			return Vector2.Zero;
		}

		return new Vector2(this.GetDirectionToTarget().X, 0.0f).Normalized();
	}

	protected override Vector2 GetMovementDirection_Attacking()
	{
		return GetMovementDirection_Chase();
	}

    protected override bool IsJumping() => false;

	protected override void ProcessAction_Idle()
	{
		if (Target == null)
		{
			return;
		}
		else if (this.IsTargetInFieldOfView(FaceDirection, FieldOfView) && this.IsTargetInLineOfSight())
		{
			State = EnemyState.Chase;

			lastTimePlayerSeen = DateTime.Now;
		}
	}

	protected override void ProcessAction_Chase()
	{
		if (Target == null)
		{
			State = EnemyState.Idle;
		}
		else if (this.IsTargetInLineOfSight())
		{
			State = EnemyState.Attacking;

			lastTimePlayerSeen = DateTime.Now;
		}
		else if ((DateTime.Now - lastTimePlayerSeen).Seconds >= chaseDuration)
		{
			State = EnemyState.Idle;
		}
	}

	protected override async void ProcessAction_Attacking()
	{
		if (Target == null)
		{
			State = EnemyState.Idle;
		}
		else if (!this.IsTargetInLineOfSight())
		{
			State = EnemyState.Chase;

			return;
		}
		else if (attackTimer.TimeLeft > 0)
		{
			return;
		}

		attackTimer.Start();

		Missile missile = missileResource.Instantiate<Missile>();
		missile.GlobalTransform = GlobalTransform;

		await GetTree().CurrentScene.AddChildDeferred(missile);

		missile.PrimeFuse();

		missile.Track(Target, LineOfSightMask);
	}

	protected override void AnimateInjury(int damage, Vector2 globalPos, Vector2 normal)
    {
        this.EmitParticlesOnce(shrapnelSplatter.Instantiate<GpuParticles2D>(), globalPos);
    }

}
