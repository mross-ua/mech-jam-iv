using Godot;
using System;
using MechJamIV;

public partial class Barrel : ProjectileBase
{

	#region Resources

	private PackedScene shrapnelSplatter = ResourceLoader.Load<PackedScene>("res://scenes/effects/shrapnel_splatter.tscn");

	#endregion

	protected virtual async void AnimateInjury(int damage, Vector2 position, Vector2 normal)
    {
        GpuParticles2D splatter = shrapnelSplatter.Instantiate<GpuParticles2D>();
		splatter.GlobalPosition = position;
		splatter.Emitting = true;

		await GetTree().CurrentScene.AddChildDeferred(splatter);

		splatter.TimedFree(splatter.Lifetime + splatter.Lifetime * splatter.Randomness, processInPhysics:true);
    }

	#region IDestructible

	public virtual void Hurt(int damage, Vector2 globalPos, Vector2 normal)
	{
		AnimateInjury(damage, globalPos, normal);
	}

	#endregion

}
