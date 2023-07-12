using Godot;
using System;
using MechJamIV;

public partial class EnemyTroid : EnemyBase
{

    protected override Vector2 Gravity { get; set; } = Vector2.Zero;

	#region Resources

	private PackedScene acidSplatter = ResourceLoader.Load<PackedScene>("res://scenes/acid_splatter.tscn");

	#endregion

	protected override Vector2 GetMovementDirection() => GlobalTransform.Origin.DirectionTo(Player.GlobalTransform.Origin);

    protected override bool IsJumping() => false;

	protected override AttackType? IsAttacking() => null;

    protected override void ProcessAttack(double delta, AttackType attackType)
    {
        throw new NotImplementedException();
    }

	protected override void AnimateInjury(int damage, Vector2 normal)
    {
        GpuParticles2D splatter = acidSplatter.Instantiate<GpuParticles2D>();

		AddChild(splatter);

		splatter.Emitting = true;

		splatter.TimedFree(splatter.Lifetime + splatter.Lifetime * splatter.Randomness, processInPhysics:true);
    }

}
