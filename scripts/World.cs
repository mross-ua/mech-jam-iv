using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MechJamIV;

public partial class World : Node2D
{

	#region Node references

	private Player player;

	private ProgressBar healthBar;
	private GpuParticles2D immunityShield;

	private IList<Spawn> spawns;
	private Spawn activeSpawn;

	#endregion

	public override void _Ready()
	{
		player = (Player)GetTree().GetFirstNodeInGroup("player");
		player.Injured += (damage) => healthBar.Value = player.Health;
		player.Healed += (amount) => healthBar.Value = player.Health;
		player.ImmunityShieldActivated += () => immunityShield.Visible = true;
		player.ImmunityShieldDeactivated += () => immunityShield.Visible = false;

		healthBar = GetNode<ProgressBar>("Player/Camera2D/UI/HealthBar");
		healthBar.Value = player.Health;
		immunityShield = GetNode<GpuParticles2D>("Player/Camera2D/UI/HealthBar/TextureRect/ImmunityShield");

		spawns = new List<Spawn>();
		foreach (Spawn spawn in GetTree().GetNodesInGroup("spawn").OfType<Spawn>())
		{
			spawns.Add(spawn);

			if (activeSpawn == null && spawn.IsWorldSpawn)
			{
				activeSpawn = spawn;
			}

			spawn.SpawnReached += (player) =>
			{
				activeSpawn = spawn;
			};
		}

		foreach (PickupBase pickup in GetTree().GetNodesInGroup("pickup").OfType<PickupBase>())
		{
			pickup.PickedUp += () => Pickup(pickup);
		}

		foreach (EnemyBase enemy in GetTree().GetNodesInGroup("enemy").OfType<EnemyBase>())
		{
			enemy.PickupDropped += (pickup) =>
			{
				GetTree().Root.AddChild(pickup);

				pickup.PickedUp += () => Pickup(pickup);
			};
		}

		player.GlobalTransform = activeSpawn.SpawnPointMarker.GlobalTransform;
	}

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionPressed("quit"))
		{
			GetTree().Quit();
		}
		else if (Input.IsActionPressed("reset"))
		{
		}
    }

	private void Pickup(PickupBase pickup)
	{
		switch (pickup.PickupType)
		{
			case PickupType.Medkit:
				player.Heal(50);

				break;
			case PickupType.Grenade:
				player.GrenadeCount++;

				break;
		}
	}

}
