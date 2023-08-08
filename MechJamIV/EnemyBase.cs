using Godot;
using System.Collections.Generic;
using System.Linq;

namespace MechJamIV
{
    public abstract partial class EnemyBase : CharacterBase
    {

        [Signal]
        public delegate void PickupDroppedEventHandler(long pickupType);

        [Export]
        public float FieldOfView { get; set; } = 45.0f;

        [Export]
        public float CriticalHitRate { get; set; } = 0.3f;

        [Export]
        public float PickupDropRate { get; set; } = 0.5f;

        [Export]
        public int ChaseDuration { get; set; }

        public EnemyState State { get; protected set; } = EnemyState.Idle;

        #region Node references

        private readonly IList<Hitbox> hitboxes = new List<Hitbox>();

        #endregion

        public override void _Ready()
        {
            base._Ready();

            foreach (Node2D node in GetNode<Node2D>("Hitboxes").GetChildren().OfType<Node2D>())
            {
                if (node is Hitbox hitbox)
                {
                    hitboxes.Add(hitbox);

                    hitbox.Hit += (damage, isWeakSpot, position, normal) =>
                    {
                        if (isWeakSpot || GD.Randf() <= CriticalHitRate)
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
                            player.Hurt(hitbox.Damage, hitbox.GlobalPosition, Vector2.Zero);
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
                    return GetMovementDirectionForIdleState();

                case EnemyState.Chase:
                    return GetMovementDirectionForChaseState();

                case EnemyState.Attack:
                    return GetMovementDirectionForAttackState();

                default:
                    break;
            }

            return Vector2.Zero;
        }

        protected abstract Vector2 GetMovementDirectionForIdleState();

        protected abstract Vector2 GetMovementDirectionForChaseState();

        protected abstract Vector2 GetMovementDirectionForAttackState();

        protected sealed override void ProcessAction()
        {
            switch (State)
            {
                case EnemyState.Idle:
                    ProcessActionForIdleState();

                    break;
                case EnemyState.Chase:
                    ProcessActionForChaseState();

                    break;
                case EnemyState.Attack:
                    ProcessActionForAttackState();

                    break;
                default:
                    // do nothing

                    break;
            }
        }

        protected abstract void ProcessActionForIdleState();

        protected abstract void ProcessActionForChaseState();

        protected abstract void ProcessActionForAttackState();

        private void DropPickup()
        {
            PickupType? pickupType = PickupHelper.GenerateRandomPickup(PickupDropRate);

            if (pickupType.HasValue)
            {
                EmitSignal(SignalName.PickupDropped, (long)pickupType.Value);
            }
        }

        #region ICollidable

        public override void Hurt(int damage, Vector2 globalPos, Vector2 normal)
        {
            if (Health <= 0)
            {
                return;
            }

            base.Hurt(damage, globalPos, normal);

            if (Health <= 0)
            {
                DropPickup();

                foreach (Hitbox hitbox in hitboxes)
                {
                    hitbox.QueueFree();
                }

                hitboxes.Clear();
            }
        }

        #endregion

    }
}