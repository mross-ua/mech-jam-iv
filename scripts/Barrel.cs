using Godot;
using System;

public partial class Barrel : RigidBody2D
{

	[Signal]
	public delegate void HurtEventHandler(int damage);

	#region Resources

	private PackedScene shrapnelSplatter = ResourceLoader.Load<PackedScene>("res://scenes/shrapnel_splatter.tscn");

	#endregion

	protected virtual async System.Threading.Tasks.Task AnimateInjuryAsync(int damage, Vector2 normal)
    {
        GpuParticles2D splatter = shrapnelSplatter.Instantiate<GpuParticles2D>();

		AddChild(splatter);

		splatter.Emitting = true;

		await ToSignal(GetTree().CreateTimer(5.0f), SceneTreeTimer.SignalName.Timeout);

		splatter.QueueFree();
    }

	public virtual async System.Threading.Tasks.Task HurtAsync(int damage, Vector2 normal)
	{
		await AnimateInjuryAsync(damage, normal);
	}

}
