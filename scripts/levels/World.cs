using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MechJamIV;

public partial class World : Node2D
{

	[Export]
	public PackedScene NextScene { get; set; } = null;

	#region Node references

	private Player player;
	private Robot robot;

	private PlayerCamera playerCamera;

	private IList<Spawn> spawns;
	private Spawn activeSpawn;

	private PauseScreen pauseScreen;

	#endregion

	public override void _Ready()
	{
		player = (Player)GetTree().GetFirstNodeInGroup("player");

		robot = (Robot)GetTree().GetFirstNodeInGroup("robot");

		playerCamera = GetNode<PlayerCamera>("PlayerCamera");
		pauseScreen = GetNode<PauseScreen>("PauseScreen");

		InitSpawns();
		InitPickups();
		InitEnemies();
		InitObjectives();
		InitDeathZones();

		player.GlobalTransform = activeSpawn.SpawnPointMarker.GlobalTransform;
		player.Killed += () => pauseScreen.PauseGame();

		robot.GlobalTransform = player.RobotMarker.GlobalTransform;
		robot.CharacterTracker.Track(player);

		playerCamera.Track(player);
	}

	private void InitSpawns()
	{
		spawns = new List<Spawn>();
		foreach (Spawn spawn in GetTree().GetNodesInGroup("spawn").OfType<Spawn>())
		{
			spawns.Add(spawn);

			if (activeSpawn == null || spawn.IsWorldSpawn)
			{
				activeSpawn = spawn;
			}

			spawn.SpawnReached += (player) =>
			{
				activeSpawn = spawn;
			};
		}
	}

	private void InitPickups()
	{
		foreach (PickupBase pickup in GetTree().GetNodesInGroup("pickup").OfType<PickupBase>())
		{
			pickup.PickedUp += () => player.Pickup(pickup);
		}
	}

	private void InitEnemies()
	{
		foreach (EnemyBase enemy in GetTree().GetNodesInGroup("enemy").OfType<EnemyBase>())
		{
			enemy.PickupDropped += (pickup) =>
			{
				pickup.PickedUp += () => player.Pickup(pickup);

				GetTree().CurrentScene.AddChildDeferred(pickup);
			};

			enemy.CharacterTracker.Track(player);
		}
	}

	private void InitObjectives()
	{
		foreach (Objective objective in GetTree().GetNodesInGroup("objective").OfType<Objective>())
		{
			objective.ObjectiveReached += () =>
			{
				GetTree().ChangeSceneToPacked(NextScene);
			};
		}
	}

	private void InitDeathZones()
	{
		foreach (Area2D deathzone in GetTree().GetNodesInGroup("deathzone").OfType<Area2D>())
		{
			deathzone.BodyEntered += (body) =>
			{
				if (body is Player player)
				{
					player.Hurt(player.Health, player.GlobalTransform.Origin, Vector2.Zero);
				}
			};
		}
	}

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("quit"))
		{
			pauseScreen.PauseGame();
		}
		else if (Input.IsActionJustPressed("fire_secondary"))
		{
			player.Fire(FireMode.Secondary, GetGlobalMousePosition());
		}
		else if (Input.IsActionJustPressed("fire_primary"))
		{
			player.Fire(FireMode.Primary, GetGlobalMousePosition());
		}
		else if (Input.IsActionPressed("fire_primary"))
		{
			player.Fire(FireMode.Primary, GetGlobalMousePosition());
		}
    }

}
