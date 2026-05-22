using Godot;
using System;

namespace MechJamIV;

public partial class Spikes : Area2D
{

    public override void _Ready()
    {
        BodyEntered += (body) =>
        {
            if (body is Player player)
            {
                player.Hurt(ConfigManager.SpikeDamage, player.GlobalPosition, Vector2.Zero);
            }
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        foreach (Node2D node in GetOverlappingBodies())
        {
            if (node is Player player)
            {
                player.Hurt(ConfigManager.SpikeDamage, player.GlobalPosition, Vector2.Zero);
            }
        }
    }

}
