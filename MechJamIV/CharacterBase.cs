using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

namespace MechJamIV {
	public abstract partial class CharacterBase : CharacterBody2D
        ,IDestructible
	{

        [Export]
        public Vector2 FaceDirection { get; set; } = Vector2.Zero;
		[Export]
		public float MoveAcceleration { get; set; } = 1.0f;
		[Export]
		public float MaxMoveSpeed { get; set; } = 10.0f;
        [Export]
        public float JumpVelocity { get; set; } = -10.0f;

	    protected virtual Vector2 Gravity { get; set; } = ProjectSettings.GetSetting("physics/2d/default_gravity_vector").AsVector2().Normalized() * ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
		protected virtual Vector2 Drag { get; set; } = Vector2.Zero;

        #region Node references

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

            characterAnimator = GetNode<CharacterAnimator>("CharacterAnimator");
		    collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
		}

		protected abstract Vector2 GetMovementDirection();

		protected abstract bool IsJumping();

        public override void _Process(double delta)
        {
            if (Health <= 0)
            {
                return;
            }

            AnimateMovement();

#if DEBUG
            QueueRedraw();
#endif
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

			if (IsJumping())
			{
				Velocity += new Vector2(0.0f, JumpVelocity);
			}
		}

        protected virtual void ProcessAction()
        {

        }

        protected void AnimateMovement() => characterAnimator.AnimateMovement(FaceDirection);

        protected abstract void AnimateInjury(int damage, Vector2 position, Vector2 normal);

        protected void AnimateDeath() => characterAnimator.AnimateDeath();

        #region ICollidable/IDestructible

        [Signal]
        public delegate void InjuredEventHandler(int damage);
        [Signal]
	    public delegate void KilledEventHandler();
        [Signal]
        public delegate void HealedEventHandler(int health);

        [Export]
        public int Health { get; set; } = 100;

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

			    collisionShape2D.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

                //TODO the game is reacting poorly when we free the player
                //this.TimedFree(5.0f, processInPhysics:true);
            }
        }

		public virtual void Heal(int health)
		{
            if (Health <= 0)
            {
                return;
            }

            //TODO we need to know initial/max health
            Health = Math.Min(100, Health + health);

            EmitSignal(SignalName.Healed, health);
		}

        #endregion

	}
}
