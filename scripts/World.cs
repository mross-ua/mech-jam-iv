using Godot;
using System;

public partial class World : Node2D
{

	Player player;

	public override void _Ready()
	{
		player = GetNode<Player>("Player");
	}

	public override void _Process(double delta)
	{

	}

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("quit"))
		{
			GetTree().Quit();
		}
    }

}
