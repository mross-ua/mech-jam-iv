using Godot;
using System;
using System.Collections.Generic;

namespace MechJamIV {
	public abstract partial class CharacterBase : CharacterBody2D
	{

        [Signal]
        public delegate void HealEventHandler(int hp);
        [Signal]
        public delegate void HurtEventHandler(int damage);

        [Export]
        public int Health { get; set; } = 100;

		[Export]
		public virtual float MoveAcceleration { get; set; } = 50.0f;
		[Export]
		public virtual float MaxMoveSpeed { get; set; } = 300.0f;
        [Export]
        public float JumpVelocity { get; set; } = -400.0f;

	    protected virtual Vector2 Gravity { get; set; } = ProjectSettings.GetSetting("physics/2d/default_gravity_vector").AsVector2().Normalized() * ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
		protected virtual Vector2 Drag { get; set; } = Vector2.Zero;

        #region Node references

        private CharacterAnimator characterAnimator;

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
		}

		protected abstract Vector2 GetMovementDirection();

		protected abstract bool IsJumping();

		protected abstract bool IsAttacking();

        public override void _Process(double delta)
        {
            QueueRedraw();
        }

        public override void _Draw()
        {
            DrawLine(Vector2.Zero, GetMovementDirection() * 25, Colors.Red);
        }

		public override void _PhysicsProcess(double delta)
		{
            if (Health <= 0)
            {
                return;
            }

			if (IsAttacking())
            {
                ProcessAttack(delta);
            }

            Vector2 direction = GetMovementDirection();

            AnimateMovement(direction, delta);

			Velocity += MoveAcceleration * direction - Drag * Velocity + (float)delta * Gravity;

			MoveAndSlide();

			if (IsJumping())
			{
				Velocity += new Vector2(0.0f, JumpVelocity);
			}
		}

        protected abstract void ProcessAttack(double delta);

        protected void AnimateMovement(Vector2 direction, double delta) => characterAnimator.AnimateMovement(direction, delta);

        protected void AnimateDeath() => characterAnimator.AnimateDeath();

        public virtual async void HurtAsync(int damage, Vector2 normal)
        {
            if (Health <= 0)
            {
                return;
            }

            Health = Math.Max(0, Health - damage);

            EmitSignal(SignalName.Hurt, damage);

            if (Health <= 0)
            {
                AnimateDeath();

                //TODO the game is reacting poorly when we free the player
                //await ToSignal(GetTree().CreateTimer(5.0f), SceneTreeTimer.SignalName.Timeout);

			    //QueueFree();
            }
        }

		public virtual async void HealAsync(int amount)
		{
            if (Health <= 0)
            {
                return;
            }

            //TODO we need to know initial/max health
            Health = Math.Min(100, Health + amount);

            EmitSignal(SignalName.Heal, amount);
		}

	}
}
