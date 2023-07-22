using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MechJamIV {
    public abstract partial class EnemyBase : CharacterBase
        ,ITracker
    {

        [Signal]
        public delegate void PickupDroppedEventHandler(PickupBase pickup);

        [Export]
        public float FieldOfView { get; set; } = 45.0f;
        [Export]
        public float CriticalHitRate { get; set; } = 0.3f;
        [Export]
        public float PickupDropRate { get; set; } = 0.5f;

        public EnemyState State { get; protected set; } = EnemyState.Idle;

        public override void _Ready()
        {
            base._Ready();

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

#if DEBUG
            AddRayCast();
#endif
        }

        protected sealed override Vector2 GetMovementDirection()
        {
            switch (State)
            {
                case EnemyState.Idle:
                    return GetMovementDirection_Idle();

                case EnemyState.Chase:
                    return GetMovementDirection_Chase();

                case EnemyState.Attacking:
                    return GetMovementDirection_Attacking();
            }

            return Vector2.Zero;
        }

        protected abstract Vector2 GetMovementDirection_Idle();

        protected abstract Vector2 GetMovementDirection_Chase();

        protected abstract Vector2 GetMovementDirection_Attacking();

        protected sealed override void ProcessAction()
        {
            switch (State)
            {
                case EnemyState.Idle:
                    ProcessAction_Idle();

                    break;
                case EnemyState.Chase:
                    ProcessAction_Chase();

                    break;
                case EnemyState.Attacking:
                    ProcessAction_Attacking();

                    break;
            }

#if DEBUG
            UpdateRayCastToTarget();
#endif
        }

        protected abstract void ProcessAction_Idle();

        protected abstract void ProcessAction_Chase();

        protected abstract void ProcessAction_Attacking();

        private void DropPickup()
        {
            PickupBase pickup = PickupHelper.GenerateRandomPickup(PickupDropRate);

            if (pickup != null)
            {
                pickup.GlobalTransform = GlobalTransform;

                EmitSignal(SignalName.PickupDropped, pickup);
            }
        }

        protected Vector2 GetDirectionToTarget()
        {
            Debug.Assert(Target != null, "A target is not currently being tracked.");

            return GlobalTransform.Origin.DirectionTo(Target.GlobalTransform.Origin);
        }

        protected bool IsTargetInFieldOfView()
        {
            return Mathf.RadToDeg(FaceDirection.AngleTo(GetDirectionToTarget())) < FieldOfView;
        }

        protected bool IsTargetInLineOfSight()
        {
            Debug.Assert(Target != null, "A target is not currently being tracked.");

            Godot.Collections.Dictionary collision = GetWorld2D().DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters2D()
            {
                From = GlobalTransform.Origin + Vector2.Up, // offset so we don't collide with ground
                To = Target.GlobalTransform.Origin,
                Exclude = null,
                CollideWithBodies = true,
                CollideWithAreas = true,
                CollisionMask = (uint)(CollisionLayerMask.World | CollisionLayerMask.Player)
            });

            return collision.ContainsKey("collider") && collision["collider"].Obj == Target;
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
            }
        }

        #endregion

        #region ITracker

        public CharacterBase Target { get; private set; }

        public void Track(CharacterBase c)
        {
            Target = c;

            Target.Killed += () => Untrack(c);
            // just in case we miss the Killed signal
            Target.TreeExiting += () => Untrack(c);
        }

        private void Untrack(CharacterBase c)
        {
            // make sure we are still tracking the object that fired this event
            if (Target == c)
            {
                Target = null;
            }
        }

        #endregion

    }
}