using Godot;
using System;
using MechJamIV;

public partial class EnemyMech : EnemyBase
{

    public override Vector2 FaceDirection { get; set; } = Vector2.Left;

	#region Resources

	private PackedScene shrapnelSplatter = ResourceLoader.Load<PackedScene>("res://scenes/shrapnel_splatter.tscn");

	#endregion

	public override void _Ready()
	{
		base._Ready();
	}

	protected override Vector2 GetMovementDirection_Idle()
	{
		return Vector2.Zero;
	}

	protected override Vector2 GetMovementDirection_Chasing()
	{
		return GetMovementDirection_Idle();
	}

	protected override Vector2 GetMovementDirection_Attacking()
	{
		return GetMovementDirection_Idle();
	}

    protected override bool IsJumping() => false;

    protected override void ProcessAttack(double delta)
    {
        //TODO
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
