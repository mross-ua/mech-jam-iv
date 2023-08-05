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

	private bool isEnteringTargetMode = false;

	#endregion

	public override void _Ready()
	{
	    Input.SetCustomMouseCursor(ResourceLoader.Load<CompressedTexture2D>("res://assets/sprites/WhiteCrosshair-5.png"), Input.CursorShape.Arrow, new Vector2(32.0f, 32.0f));

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
		player.Killed += async () =>
		{
			//TODO ideally this should wait for the death animation to finish
			await ToSignal(GetTree().CreateTimer(3.0f, false, true), SceneTreeTimer.SignalName.Timeout);

			pauseScreen.PauseGame();
		};

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
			pickup.PickedUp += () => Pickup(pickup);
		}
	}

	private void InitEnemies()
	{
		foreach (EnemyBase enemy in GetTree().GetNodesInGroup("enemy").OfType<EnemyBase>())
		{
			enemy.PickupDropped += (pickup) =>
			{
				pickup.PickedUp += () => Pickup(pickup);

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
					player.Hurt(player.Health, player.GlobalPosition, Vector2.Zero);
				}
			};
		}
	}
    public override void _Input(InputEvent @event)
    {
		if (@event.IsActionPressed("quit"))
		{
			pauseScreen.PauseGame();

			GetViewport().SetInputAsHandled();
		}
    }

    public override void _PhysicsProcess(double delta)
    {
		if (Input.IsActionPressed("fire_secondary"))
		{
			if (player.CharacterTracker.Target == null || !isEnteringTargetMode)
			{
				CollisionObject2D target = FindTarget(GetGlobalMousePosition());

				if (target != null && target != player.CharacterTracker.Target)
				{
					player.CharacterTracker.Track(target);

					isEnteringTargetMode = true;
				}
			}
		}
		else if (Input.IsActionJustReleased("fire_secondary"))
		{
			if (isEnteringTargetMode)
			{
				isEnteringTargetMode = false;
			}
			else
			{
				player.Fire(FireMode.Secondary, GetGlobalMousePosition());
			}
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

	private void Pickup(PickupBase pickup)
	{
		switch (pickup.PickupType)
		{
			case PickupType.Medkit:
				player.Heal(50);

				break;
			case PickupType.Rifle:
			case PickupType.Grenade:
			case PickupType.Missile:
				player.WeaponManager.Pickup(pickup.PickupType);

				break;
		}
	}

	private CollisionObject2D FindTarget(Vector2 globalPos)
	{
		CollisionObject2D target = null;

		PhysicsShapeQueryParameters2D queryParams = new ()
		{
			Transform = new Transform2D()
			{
				Origin = globalPos
			},
			Shape = new CircleShape2D()
			{
				Radius = 300.0f
			},
			CollisionMask = (uint)CollisionLayerMask.Enemy,
			Exclude = null
		};

		foreach (Godot.Collections.Dictionary collision in GetWorld2D().DirectSpaceState.IntersectShape(queryParams))
		{
			if (collision["collider"].Obj is CharacterBase character)
			{
				if (target == null || player.CharacterTracker.Target != character)
				{
					target = character;
				}
				else if ((character.GlobalPosition - GlobalPosition).Length() < (target.GlobalPosition - GlobalPosition).Length())
				{
					target = character;
				}
			}
		}

		return target;
	}

}
