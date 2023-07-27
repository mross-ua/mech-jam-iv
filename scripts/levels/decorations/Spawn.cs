using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MechJamIV;

public partial class Spawn : Node2D
{

	[Signal]
	public delegate void SpawnReachedEventHandler(Player player);

	[Export]
	public bool IsWorldSpawn { get; set; } = false;

	#region Node references

	public Marker2D SpawnPointMarker { get; set; }

	private Timer healthGenTimer;
	private Area2D area2D;
	private CollisionShape2D collisionShape2D;
	private GpuParticles2D gpuParticles2D;

	#endregion

	public override void _Ready()
	{
		SpawnPointMarker = GetNode<Marker2D>("SpawnPoint");

		healthGenTimer = GetNode<Timer>("HealthGenTimer");
		healthGenTimer.Timeout += () => Generate();

		area2D = GetNode<Area2D>("Area2D");
		area2D.BodyEntered += (body) =>
		{
			if (body is Player player)
			{
				gpuParticles2D.Visible = true;

				if (healthGenTimer.IsStopped())
				{
					healthGenTimer.Start();
				}

				EmitSignal(SignalName.SpawnReached, player);
			}
		};
		area2D.BodyExited += (body) =>
		{
			if (body is Player player)
			{
				if (!area2D.GetOverlappingBodies().OfType<Player>().Any())
				{
					gpuParticles2D.Visible = false;

					healthGenTimer.Stop();
				}
			}
		};

		collisionShape2D = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
		gpuParticles2D = GetNode<GpuParticles2D>("GPUParticles2D");
	}

	private void Generate()
	{
		foreach (Node2D body in area2D.GetOverlappingBodies())
		{
			if (body is Player player)
			{
				player.Heal(10);
			}
		}
	}

}
