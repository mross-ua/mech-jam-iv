using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MechJamIV.Base;
using MechJamIV.Enums;
using MechJamIV.Extensions;
using MechJamIV.Interfaces;
using MechJamIV.Levels;

namespace MechJamIV;

public partial class World : Node2D,
    IUpdateable
{

    [Signal]
    public delegate void SpawnReachedEventHandler(Player player);

    [Export(PropertyHint.File, "*.tscn,")]
    public string NextScene { get; set; } = null!;

    private int numObjectivesRemaining = 0;

    #region Node references

    private Player player = null!;
    private Robot robot = null!;

    private PlayerCamera playerCamera = null!;

    private readonly IList<Spawn> spawns = [];
    private Spawn activeSpawn = null!;

    private PauseScreen pauseScreen = null!;

    private bool isEnteringTargetMode = false;

    #endregion

    public override void _Ready()
    {
        Input.SetCustomMouseCursor(ResourceManager.CursorTexture, Input.CursorShape.Arrow, new Vector2(32.0f, 32.0f));

        player = this.FindChildrenInGroup("player").OfType<Player>().First();

        robot = this.FindChildrenInGroup("robot").OfType<Robot>().First();

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
            //TODO ideally this should wait for the death animation to finish (using a signal)
            await ToSignal(GetTree().CreateTimer(3.0f, false, true), SceneTreeTimer.SignalName.Timeout);

            pauseScreen.PauseGame();
        };

        robot.GlobalTransform = player.RobotMarker.GlobalTransform;
        robot.CharacterTracker!.Track(player);

        playerCamera.Track(player);

        SceneManager.UnpauseGame();
    }

    private void InitSpawns()
    {
        foreach (Spawn spawn in GetSpawns())
        {
            spawns.Add(spawn);

            if (activeSpawn == null || spawn.IsWorldSpawn)
            {
                activeSpawn = spawn;
            }

            spawn.SpawnReached += (player) =>
            {
                activeSpawn = spawn;

                EmitSignal(SignalName.SpawnReached, player);
            };
        }
    }

    private void InitPickups()
    {
        foreach (Node node in GetPickups())
        {
            if (node is Projectile p)
            {
                p.PickedUp += () => Pickup(p.WeaponType);
            }
            else if (node is HitScanBulletEmitterPickup ep)
            {
                ep.PickedUp += () => Pickup(PickupType.Rifle);
            }
        }
    }

    private void InitEnemies()
    {
        foreach (EnemyBase enemy in GetEnemies())
        {
            enemy.PickupDropped += (pickupType) => DropPickup((PickupType)pickupType, enemy.GlobalPosition);

            enemy.CharacterTracker?.Track(player);
        }
    }

    private void InitObjectives()
    {
        foreach (Objective objective in GetObjectives())
        {
            if (objective is CyberSteel cyberSteel)
            {
                numObjectivesRemaining++;
            }

            objective.ObjectiveReached += () =>
            {
                if (objective is CyberSteel cyberSteel)
                {
                    numObjectivesRemaining--;
                }
                else if (objective is Spaceship spaceship)
                {
                    if (numObjectivesRemaining == 0)
                    {
                        SceneManager.GoToScene(NextScene);
                    }
                }
            };
        }
    }

    private void InitDeathZones()
    {
        foreach (Area2D deathzone in GetDeathZones())
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

    private IEnumerable<Spawn> GetSpawns()
    {
        return this.FindChildrenInGroup("spawn").Where(n => !n.IsQueuedForDeletion()).OfType<Spawn>();
    }

    private IEnumerable<Node> GetPickups()
    {
        return this.FindChildrenInGroup("pickup").Where(n => !n.IsQueuedForDeletion());
    }

    private IEnumerable<EnemyBase> GetEnemies()
    {
        return this.FindChildrenInGroup("enemy").Where(n => !n.IsQueuedForDeletion()).OfType<EnemyBase>();
    }

    private IEnumerable<Objective> GetObjectives()
    {
        return this.FindChildrenInGroup("objective").Where(n => !n.IsQueuedForDeletion()).OfType<Objective>();
    }

    private IEnumerable<Area2D> GetDeathZones()
    {
        return this.FindChildrenInGroup("deathzone").Where(n => !n.IsQueuedForDeletion()).OfType<Area2D>();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("quit"))
        {
            pauseScreen.PauseGame();

            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionPressed("next_weapon_primary"))
        {
            player.WeaponManager.NextWeaponPrimary();
        }
        else if (@event.IsActionPressed("next_weapon_secondary"))
        {
            player.WeaponManager.NextWeaponSecondary();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionPressed("fire_secondary"))
        {
            if (player.CharacterTracker!.Target is null || !isEnteringTargetMode)
            {
                PhysicsBody2D? target = FindTarget(GetGlobalMousePosition());

                if (target is not null && target != player.CharacterTracker.Target)
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

    private void Pickup(PickupType pickupType)
    {
        switch (pickupType)
        {
            case PickupType.Medkit:
                player.Heal(ConfigManager.MedkitHealth, true);

                break;
            case PickupType.Rifle:
            case PickupType.Grenade:
            case PickupType.Missile:
                player.WeaponManager.CallDeferred(WeaponManager.MethodName.DeferredPickup, (long)pickupType);

                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void DropPickup(PickupType pickupType, Vector2 globalPos)
    {
        Projectile projectile = ResourceManager.GenerateProjectile(pickupType);
        projectile.GlobalPosition = globalPos;

        projectile.PickedUp += () => Pickup(projectile.WeaponType);

        GetTree().CurrentScene.AddChildDeferred(projectile);
    }

    private PhysicsBody2D? FindTarget(Vector2 globalPos)
    {
        PhysicsBody2D? target = null;

        PhysicsShapeQueryParameters2D queryParams = new()
        {
            Transform = new Transform2D()
            {
                Origin = globalPos
            },
            Shape = new CircleShape2D()
            {
                Radius = ConfigManager.MouseSensitivity
            },
            CollisionMask = (uint)CollisionLayerMask.Enemy,
            Exclude = null
        };

        foreach (Godot.Collections.Dictionary collision in GetWorld2D().DirectSpaceState.IntersectShape(queryParams))
        {
            if (collision["collider"].Obj is CharacterBase character)
            {
                if (target is null || player.CharacterTracker!.Target != character)
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

    #region IUpdateable

    public void Save(ConfigFile config)
    {
        config.SetValue(nameof(World), nameof(Spawn.SpawnPointMarker), activeSpawn.SpawnPointMarker.GlobalTransform);

        player.Save(config);
    }

    public void DeferredLoad(ConfigFile config)
    {
        if (config.HasSectionKey(nameof(World), nameof(Spawn.SpawnPointMarker)))
        {
            player.GlobalTransform = config.GetValue(nameof(World), nameof(Spawn.SpawnPointMarker)).AsTransform2D();
            // reunite Robot with Player
            robot.GlobalTransform = config.GetValue(nameof(World), nameof(Spawn.SpawnPointMarker)).AsTransform2D();
        }

        player.DeferredLoad(config);
    }

    #endregion

}
