using Godot;
using System;

namespace MechJamIV.Extensions;

public static class ParticleHelper
{

    public static async void EmitParticlesOnce(this Node node, GpuParticles2D particles, Vector2 globalPos)
    {
        particles.GlobalPosition = globalPos;

        particles.Emitting = true;

        // this adds to the top-level scene!
        await node.GetTree().CurrentScene.AddChildDeferred(particles);

        particles.TimedFree(particles.Lifetime + (particles.Lifetime * particles.Randomness));
    }

}
