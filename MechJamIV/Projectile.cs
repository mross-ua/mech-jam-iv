using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MechJamIV;

namespace MechJamIV {
    public partial class Projectile : RigidBody2D
        ,ICollidable
    {

        [Signal]
        public delegate void PickedUpEventHandler();

        [Export]
        public PackedScene PointDamageEffect { get; set; }

        [Export(PropertyHint.Enum)]
        public PickupType WeaponType { get; set; }

        [Export]
        public Texture2D UISprite { get; set; }

        public override void _Ready()
        {
            BodyEntered += (body) =>
            {
                if (CanBePickedUp() && body is Player player)
                {
                    EmitSignal(SignalName.PickedUp);

                    QueueFree();
                }
            };
        }

        protected virtual bool CanBePickedUp() => true;

        public void SetBodiesToExclude(IEnumerable<PhysicsBody2D> bodies)
        {
            foreach (PhysicsBody2D body in GetCollisionExceptions())
            {
                RemoveCollisionExceptionWith(body);
            }

            foreach (PhysicsBody2D body in bodies)
            {
                AddCollisionExceptionWith(body);
            }
        }

        private void AnimateInjury(Vector2 globalPos)
        {
            this.EmitParticlesOnce(PointDamageEffect.Instantiate<GpuParticles2D>(), globalPos);
        }

        #region ICollidable

        [Signal]
        public delegate void InjuredEventHandler(int damage);

        public virtual void Hurt(int damage, Vector2 globalPos, Vector2 normal)
        {
            AnimateInjury(globalPos);
        }

        #endregion

    }
}
