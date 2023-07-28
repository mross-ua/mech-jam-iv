using Godot;
using System;
using MechJamIV;

public partial class Barrel : ProjectileBase
{

	protected virtual void AnimateInjury(int damage, Vector2 globalPos, Vector2 normal)
    {
        this.EmitParticlesOnce(PointDamageEffect.Instantiate<GpuParticles2D>(), globalPos);
    }

	#region ICollidable

	public override void Hurt(int damage, Vector2 globalPos, Vector2 normal)
	{
		AnimateInjury(damage, globalPos, normal);
	}

	#endregion

}
