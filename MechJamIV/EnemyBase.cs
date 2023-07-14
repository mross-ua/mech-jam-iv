using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MechJamIV {
    public abstract partial class EnemyBase : CharacterBase
    {

        [Signal]
        public delegate void PickupDroppedEventHandler(PickupBase pickup);

        [Export]
        public float CriticalHitRate { get; set; } = 0.3f;
        [Export]
        public float PickupDropRate { get; set; } = 0.5f;

        public EnemyState State { get; protected set; } = EnemyState.Idle;

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
                    hitbox.Hit += (damage, isWeakSpot, position, normal) =>
                    {
                        if (isWeakSpot || RandomHelper.GetSingle() <= CriticalHitRate)
                        {
                            Hurt(damage * 2, position, normal);
                        }
                        else
                        {
                            Hurt(damage, position, normal);
                        }
                    };
                    hitbox.Colliding += (body) =>
                    {
                        if (Health <= 0)
                        {
                            return;
                        }

                        if (body is Player player)
                        {
                            player.Hurt(hitbox.Damage, hitbox.GlobalTransform.Origin, Vector2.Zero);
                        }
                    };
                }
            }
        }

        protected sealed override Vector2 GetMovementDirection()
        {
            switch (State)
            {
                case EnemyState.Idle:
                    return GetMovementDirection_Idle();

                case EnemyState.Chasing:
                    return GetMovementDirection_Chasing();

                case EnemyState.Attacking:
                    return GetMovementDirection_Attacking();
            }

            return Vector2.Zero;
        }

        protected abstract Vector2 GetMovementDirection_Idle();

        protected abstract Vector2 GetMovementDirection_Chasing();

        protected abstract Vector2 GetMovementDirection_Attacking();

        public override void Hurt(int damage, Vector2 position, Vector2 normal)
        {
            if (Health <= 0)
            {
                return;
            }

            base.Hurt(damage, position, normal);

            if (Health <= 0)
            {
                PickupBase pickup = PickupHelper.GenerateRandomPickup(PickupDropRate);

                if (pickup != null)
                {
                    pickup.GlobalTransform = GlobalTransform;

                    EmitSignal(SignalName.PickupDropped, pickup);
                }

                this.TimedFree(5.0f, processInPhysics:true);
            }
        }

    }
}