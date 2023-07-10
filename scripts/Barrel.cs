using Godot;
using System;

public partial class Barrel : RigidBody2D
{

	[Signal]
	public delegate void HurtEventHandler(int damage);

	[Export]
	public int Health { get; set; } = 10;

	#region Resourcesvirtual

	private PackedScene shrapnelSplatter = ResourceLoader.Load<PackedScene>("res://scenes/shrapnel_splatter.tscn");

	#endregion

	public override void _Process(double delta)
	{

	}

	protected async void AnimateInjuryAsync(int damage, Vector2 normal)
    {
        GpuParticles2D splatter = shrapnelSplatter.Instantiate<GpuParticles2D>();

		AddChild(splatter);

		splatter.Emitting = true;

		await ToSignal(GetTree().CreateTimer(5.0f), SceneTreeTimer.SignalName.Timeout);

		splatter.QueueFree();
    }

	protected virtual void AnimateDeath()
	{
		//TODO
	}

	public virtual async void HurtAsync(int damage, Vector2 normal)
	{
		if (Health <= 0)
		{
			return;
		}

		Health = Math.Max(0, Health - damage);

		EmitSignal(SignalName.Hurt, damage);

		AnimateInjuryAsync(damage, normal);

		if (Health <= 0)
		{
			AnimateDeath();

			await ToSignal(GetTree().CreateTimer(5.0f), SceneTreeTimer.SignalName.Timeout);

			QueueFree();
		}
	}

}
