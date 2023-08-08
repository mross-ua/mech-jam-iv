using Godot;
using System;

namespace MechJamIV
{
    public abstract partial class CharacterBase : CharacterBody2D
        , IDestructible
    {

        [Export]
        public Vector2 FaceDirection { get; set; } = Vector2.Zero;

        [Export]
        public float MoveAcceleration { get; set; } = 1.0f;

        [Export]
        public float MaxMoveSpeed { get; set; } = 10.0f;

        [Export]
        public float JumpVelocity { get; set; } = -10.0f;

        [Export]
        public float MaxJumpAirTime { get; set; }

        [Export]
        public PackedScene PointDamageEffect { get; set; }

        protected virtual Vector2 Gravity { get; set; } = ProjectSettings.GetSetting("physics/2d/default_gravity_vector").AsVector2().Normalized() * ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
        protected virtual Vector2 Drag { get; set; } = Vector2.Zero;

        private double jumpAirTime = 0.0f;
        private bool isJumping = false;

        #region Node references

        public CharacterTracker CharacterTracker { get; private set; }

        private CharacterAnimator characterAnimator;

        private CollisionShape2D collisionShape2D;

        #endregion

        public override void _Ready()
        {
            if (MotionMode == MotionModeEnum.Grounded)
            {
                Drag = new Vector2(MoveAcceleration / MaxMoveSpeed, 0.0f);
            }
            else
            {
                Drag = new Vector2(MoveAcceleration / MaxMoveSpeed, MoveAcceleration / MaxMoveSpeed);
            }

            CharacterTracker = GetNodeOrNull<CharacterTracker>("CharacterTracker");

            characterAnimator = GetNode<CharacterAnimator>("CharacterAnimator");
            collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        }

        protected abstract Vector2 GetMovementDirection();

        private bool IsJumping(double delta)
        {
            if (jumpAirTime >= MaxJumpAirTime && _IsJumping())
            {
                jumpAirTime = MaxJumpAirTime;
                isJumping = false;
            }
            else if (isJumping && jumpAirTime < MaxJumpAirTime && _IsJumping())
            {
                jumpAirTime += delta;
                isJumping = true;
            }
            else if (isJumping)
            {
                jumpAirTime = 0.0f;
                isJumping = false;
            }
            else if (!isJumping)
            {
                jumpAirTime = 0.0f;
                isJumping = _IsJumping();
            }

            return isJumping;
        }

        protected abstract bool _IsJumping();

        public override void _Process(double delta)
        {
            if (Health <= 0)
            {
                return;
            }

            AnimateMovement();
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Health <= 0)
            {
                return;
            }

            FaceDirection = GetMovementDirection();

            ProcessAction();

            Velocity += MoveAcceleration * FaceDirection - Drag * Velocity + (float)delta * Gravity;

            MoveAndSlide();

            if (IsJumping(delta))
            {
                Velocity += new Vector2(0.0f, JumpVelocity * (1.0f - (float)(jumpAirTime / MaxJumpAirTime))) - (float)delta * Gravity;
            }
        }

        protected virtual void ProcessAction()
        {

        }

        protected void AnimateMovement() => characterAnimator.AnimateMovement(FaceDirection);

        protected abstract void AnimateInjury(int damage, Vector2 position, Vector2 normal);

        protected void AnimateDeath() => characterAnimator.AnimateDeath();

        #region IDestructible/ICollidable

        [Signal]
        public delegate void InjuredEventHandler(int damage);
        [Signal]
        public delegate void KilledEventHandler();
        [Signal]
        public delegate void HealedEventHandler(int health);

        [Export]
        public int MaxHealth { get; set; }
        [Export]
        public int MaxOverHealth { get; set; }

        [Export]
        public int Health { get; set; }

        public virtual void Hurt(int damage, Vector2 globalPos, Vector2 normal)
        {
            if (Health <= 0)
            {
                return;
            }

            Health = Math.Max(0, Health - damage);

            AnimateInjury(damage, globalPos, normal);

            EmitSignal(SignalName.Injured, damage);

            if (Health <= 0)
            {
                AnimateDeath();

                EmitSignal(SignalName.Killed);

                CharacterTracker?.Untrack();

                // NOTE: We disable the collision shape and wait to
                //       free so the death animation can fully play.

                collisionShape2D.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

                this.TimedFree(5.0f);
            }
        }

        public virtual void Heal(int health, bool allowOverHealth)
        {
            if (Health <= 0)
            {
                return;
            }

            if (allowOverHealth)
            {
                Health = Math.Min(MaxHealth + MaxOverHealth, Health + health);
            }
            else
            {
                Health = Math.Min(MaxHealth, Health + health);
            }

            EmitSignal(SignalName.Healed, health);
        }

        #endregion

    }
}
