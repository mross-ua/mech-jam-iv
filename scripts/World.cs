using Godot;
using System;

public partial class World : Node2D
{

	private ProgressBar healthBar;

	public override void _Ready()
	{
		player = GetNode<Player>("Player");
		player.Heal += (hp) => healthBar.Value = player.Health;
		player.Hurt += (hp) => healthBar.Value = player.Health;

		healthBar = GetNode<ProgressBar>("Player/Camera2D/UI/HealthBar");
		healthBar.Value = player.Health;
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
