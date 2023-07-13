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

	private Dictionary<Rid, Player> collidingBodies = new ();

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
				collidingBodies.Add(player.GetRid(), player);

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
				collidingBodies.Remove(player.GetRid());

				if (!collidingBodies.Any())
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
		foreach (KeyValuePair<Rid, Player> kvp in collidingBodies)
		{
			kvp.Value.Heal(10);
		}
	}

}
