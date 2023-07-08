
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

		protected abstract Vector2 GetMovementDirection(double delta);

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
                //TODO
            }

            Vector2 direction = GetMovementDirection(delta);

            //TODO play movement animation

			Velocity += MoveAcceleration * direction - drag * Velocity + (float)delta * gravity;

			MoveAndSlide();

			if (IsJumping())
			{
				Velocity += new Vector2(0.0f, JumpVelocity);
			}
		}

        public virtual async void HurtAsync(int damage)
        {
            if (Health <= 0)
            {
                return;
            }

            Health = Math.Max(0, Health - damage);

            if (Health <= 0)
            {
                //TODO play death animation
            }

            EmitSignal(SignalName.Hurt, damage);
        }

		public virtual async void HealAsync(int amount)
		{

		}

	}
}
