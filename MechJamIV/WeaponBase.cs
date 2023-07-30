using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MechJamIV;

namespace MechJamIV {
    public abstract partial class WeaponBase : Node2D
        ,IWeapon
    {

        private bool isCoolingDown = false;

        public abstract void SetBodiesToExclude(IEnumerable<CollisionObject2D> bodies);

        protected abstract void _Fire(Vector2 globalPos, CollisionObject2D target = null);

        #region IWeapon

        private int ammo = 0;

        [Signal]
        public delegate void FiredEventHandler();

        [Export]
        public float RoundsPerSecond { get; set; }

        [Export]
        public int Ammo
        {
            get
            {
                return ammo;
            }
            set
            {
                if (ammo != value)
                {
                    ammo = value;

                    EmitSignal(SignalName.Fired);
                }
            }
        }

        [Export(PropertyHint.Layers2DPhysics)]
        public uint CollisionMask { get; set; }

        [Export]
        public float LineOfSightDistance { get; set; }

        public abstract Texture2D SpriteTexture { get; }

        public async void Fire(Vector2 globalPos, CollisionObject2D target = null)
        {
            if (isCoolingDown)
            {
                return;
            }
            else if (Ammo == 0)
            {
                return;
            }
            else if (Ammo < 0)
            {
                // allow this for infinite ammo
            }

            if (Ammo > 0)
            {
                Ammo--;
            }

            isCoolingDown = true;

            _Fire(globalPos, target);

            EmitSignal(SignalName.Fired);

            if (!Mathf.IsZeroApprox(RoundsPerSecond))
            {
                await ToSignal(GetTree().CreateTimer(1.0f / RoundsPerSecond), SceneTreeTimer.SignalName.Timeout);
            }

            isCoolingDown = false;
        }

        #endregion

    }
}