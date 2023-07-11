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
                    hitbox.Hit += async (damage, normal) => await HurtAsync(damage, normal);
                    hitbox.Colliding += async (body) =>
                    {
                        if (Health <= 0)
                        {
                            return;
                        }

                        if (body is Player player)
                        {
                            await player.HurtAsync(hitbox.Damage, Vector2.Zero);
                        }
                    };
                }
            }
        }

        public override async System.Threading.Tasks.Task HurtAsync(int damage, Vector2 normal)
        {
            if (Health <= 0)
            {
                return;
            }

            await base.HurtAsync(damage, normal);

            if (Health <= 0)
            {
                await ToSignal(GetTree().CreateTimer(5.0f), SceneTreeTimer.SignalName.Timeout);

			    QueueFree();
            }
        }

    }
}