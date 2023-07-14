using Godot;
using System;
using MechJamIV;

public partial class Barrel : RigidBody2D
{

	[Signal]
	public delegate void InjuredEventHandler(int damage);

	#region Resources

	private PackedScene shrapnelSplatter = ResourceLoader.Load<PackedScene>("res://scenes/shrapnel_splatter.tscn");

	#endregion

	protected virtual void AnimateInjury(int damage, Vector2 position, Vector2 normal)
    {
        GpuParticles2D splatter = shrapnelSplatter.Instantiate<GpuParticles2D>();

		GetTree().CurrentScene.AddChild(splatter);

		splatter.GlobalPosition = position;

		splatter.Emitting = true;

		splatter.TimedFree(splatter.Lifetime + splatter.Lifetime * splatter.Randomness, processInPhysics:true);
    }

	public virtual void Hurt(int damage, Vector2 position, Vector2 normal)
	{
		AnimateInjury(damage, position, normal);
	}

}
