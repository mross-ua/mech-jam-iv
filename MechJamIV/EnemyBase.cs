using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MechJamIV {
    public abstract partial class EnemyBase : CharacterBase
    {

        #region Node references

        protected Player Player { get; private set; }

        #endregion


        public override void _Ready()
        {
            base._Ready();

            Player = (Player)GetTree().GetFirstNodeInGroup("player");

            foreach (Node2D node in GetNode<Node2D>("Hitboxes").GetChildren())
            {
                if (node is Hitbox hitbox)
                {
                    hitbox.Hit += (damage, normal) => HurtAsync(damage, normal);
                    hitbox.Colliding += (body) =>
                    {
                        if (Health <= 0)
                        {
                            return;
                        }

                        if (body is Player player)
                        {
                            player.HurtAsync(hitbox.Damage, Vector2.Zero);
                        }
                    };
                }
            }
        }

        public override async void HurtAsync(int damage, Vector2 normal)
        {
            if (Health <= 0)
            {
                return;
            }

            base.HurtAsync(damage, normal);

            if (Health <= 0)
            {
                await ToSignal(GetTree().CreateTimer(5.0f), SceneTreeTimer.SignalName.Timeout);

			    QueueFree();
            }
        }

    }
}