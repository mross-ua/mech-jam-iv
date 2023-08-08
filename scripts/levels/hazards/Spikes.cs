using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Spikes : Area2D
{

    [Export]
    public int Damage { get; set; } = 10;

    public override void _Ready()
    {
        BodyEntered += (body) =>
        {
            if (body is Player player)
            {
                player.Hurt(Damage, player.GlobalPosition, Vector2.Zero);
            }
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        foreach (Node2D node in GetOverlappingBodies())
        {
            if (node is Player player)
            {
                player.Hurt(Damage, player.GlobalPosition, Vector2.Zero);
            }
        }
    }

}
