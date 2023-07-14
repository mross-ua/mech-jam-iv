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

    protected override void ProcessAttack(double delta)
    {
        //TODO
    }

	protected override void AnimateInjury(int damage, Vector2 position, Vector2 normal)
    {
        GpuParticles2D splatter = acidSplatter.Instantiate<GpuParticles2D>();

		GetTree().CurrentScene.AddChild(splatter);

        splatter.GlobalPosition = position;

		splatter.Emitting = true;

		splatter.TimedFree(splatter.Lifetime + splatter.Lifetime * splatter.Randomness, processInPhysics:true);
    }

}
