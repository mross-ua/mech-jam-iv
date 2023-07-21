using Godot;
using System;
using MechJamIV;

public partial class Barrel : ProjectileBase
{

	#region Resources

	private static readonly PackedScene shrapnelSplatter = ResourceLoader.Load<PackedScene>("res://scenes/effects/shrapnel_splatter.tscn");

	#endregion

	protected virtual void AnimateInjury(int damage, Vector2 globalPos, Vector2 normal)
    {
        this.EmitParticlesOnce(shrapnelSplatter.Instantiate<GpuParticles2D>(), globalPos);
    }

	#region IDestructible

	public virtual void Hurt(int damage, Vector2 globalPos, Vector2 normal)
	{
		AnimateInjury(damage, globalPos, normal);
	}

	#endregion

}
