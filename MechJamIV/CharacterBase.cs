
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

	    private Vector2 gravity = ProjectSettings.GetSetting("physics/2d/default_gravity_vector").AsVector2().Normalized() * ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
		private Vector2 drag = Vector2.Zero;

		public override void _Ready()
		{
            drag = new Vector2(MoveAcceleration / MaxMoveSpeed, 0.0f);
		}

		protected virtual Vector2 GetMovementDirection(double delta) => Vector2.Zero;

		protected virtual bool IsJumping() => false;

		protected virtual bool IsAttacking() => false;

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

            Vector2 direction = GetMovementDirection(delta);

            AnimateMovement(direction, delta);

			Velocity += MoveAcceleration * direction - drag * Velocity + (float)delta * gravity;

			MoveAndSlide();

			if (IsJumping())
			{
				Velocity += new Vector2(0.0f, JumpVelocity);
			}
		}

        protected abstract void ProcessAttack(double delta);

        protected abstract void AnimateMovement(Vector2 direction, double delta);

        protected abstract void AnimateDeath();

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

                await ToSignal(GetTree().CreateTimer(5.0f), SceneTreeTimer.SignalName.Timeout);

                //TODO the game is reacting poorly when we free the player
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
