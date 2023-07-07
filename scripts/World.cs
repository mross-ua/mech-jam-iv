using Godot;
using System;

public partial class World : Node2D
{

	private Player player;

	private ProgressBar healthBar;
	private GpuParticles2D immunityShield;

	public override void _Ready()
	{
		player = GetNode<Player>("Player");
		player.Heal += (hp) => healthBar.Value = player.Health;
		player.Hurt += (hp) => healthBar.Value = player.Health;
		player.ImmunityShieldActivated += () => immunityShield.Visible = true;
		player.ImmunityShieldDeactivated += () => immunityShield.Visible = false;

		healthBar = GetNode<ProgressBar>("Player/Camera2D/UI/HealthBar");
		healthBar.Value = player.Health;

		immunityShield = GetNode<GpuParticles2D>("Player/Camera2D/UI/HealthBar/TextureRect/ImmunityShield");
	}

	public override void _Process(double delta)
	{

	}

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionPressed("quit"))
		{
			GetTree().Quit();
		}
		else if (Input.IsActionPressed("reset"))
		{
			GetTree().ReloadCurrentScene();
		}
    }

}
