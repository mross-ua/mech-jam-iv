using Godot;
using System;

public partial class Spawn : Node2D
{

	[Signal]
	public delegate void SpawnReachedEventHandler(Player player);

	[Export]
	public bool IsWorldSpawn { get; set; } = false;

	#region Node references

	public Marker2D SpawnPointMarker { get; set; }

	private Area2D area2D;
	private GpuParticles2D gpuParticles2D;

	#endregion

	public override void _Ready()
	{
		SpawnPointMarker = GetNode<Marker2D>("SpawnPoint");

		area2D = GetNode<Area2D>("Area2D");
		area2D.BodyEntered += (body) =>
		{
			if (body is Player player)
			{
				gpuParticles2D.Visible = true;

				EmitSignal(SignalName.SpawnReached, player);
			}
		};
		area2D.BodyExited += (body) =>
		{
			if (body is Player player)
			{
				gpuParticles2D.Visible = false;
			}
		};

		gpuParticles2D = GetNode<GpuParticles2D>("GPUParticles2D");
	}

}
