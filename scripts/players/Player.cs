using Godot;
using System;
using MechJamIV.Base;
using MechJamIV.Enums;
using MechJamIV.Extensions;
using MechJamIV.Interfaces;
using System.Diagnostics;

namespace MechJamIV
{
    public partial class Player : CharacterBase,
        IPlayable,
        IUpdateable
    {

        private bool isImmune = false;
        private bool isJumping = false;

        #region Node references

        public Marker2D RobotMarker { get; private set; } = null!;
        public WeaponManager WeaponManager { get; private set; } = null!;

        private GpuParticles2D immunityShield = null!;
        private RemoteTransform2D remoteTransform = null!;

        #endregion

        public override void _Ready()
        {
            base._Ready();

            Debug.Assert(CharacterTracker is not null, $"{nameof(CharacterTracker)} must not be null");

            RobotMarker = GetNode<Marker2D>("RobotMarker");

            immunityShield = GetNode<GpuParticles2D>("ImmunityShield");

            remoteTransform = GetNode<RemoteTransform2D>("RemoteTransform");

            WeaponManager = GetNode<WeaponManager>("WeaponManager");
            WeaponManager.SetBodiesToExclude(this.Yield());
        }

        protected override Vector2 GetMovementDirection()
        {
            return Input.GetVector("move_left", "move_right", "noop", "noop");
        }

        protected override bool IsJumping()
        {
            // NOTE: This should be run in a process loop since we need user input.

            // only allow continuous jump key presses that start when character is on the floor
            // (disallow double jumps and jumps after walking off an edge)
            isJumping = (isJumping && Input.IsActionPressed("jump") && !IsOnFloor()) || (Input.IsActionJustPressed("jump") && IsOnFloor());

            return isJumping;
        }

        public void Fire(FireMode mode, Vector2 globalPos)
        {
            if (Health <= 0)
            {
                return;
            }

            WeaponManager.Fire(mode, globalPos, CharacterTracker!.Target);
        }

        public void ActivateShield()
        {
            immunityShield.Visible = true;

            EmitSignal(SignalName.ImmunityShieldActivated);
        }

        public void DeactivateShield()
        {
            immunityShield.Visible = false;

            EmitSignal(SignalName.ImmunityShieldDeactivated);
        }

        #region ICollidable

        public override async void Hurt(int damage, Vector2 globalPos, Vector2 normal)
        {
            if (isImmune)
            {
                return;
            }

            isImmune = true;

            base.Hurt(damage, globalPos, normal);

            if (Health <= 0)
            {
                return;
            }

            ActivateShield();

            await ToSignal(GetTree().CreateTimer(2.0f, false, true), SceneTreeTimer.SignalName.Timeout);

            DeactivateShield();

            isImmune = false;
        }

        #endregion

        #region IPlayable

        [Signal]
        public delegate void ImmunityShieldActivatedEventHandler();
        [Signal]
        public delegate void ImmunityShieldDeactivatedEventHandler();

        public void SetRemoteTarget(Camera2D cam)
        {
            remoteTransform.RemotePath = cam.GetPath();
        }

        #endregion

        #region IUpdateable

        [Signal]
        public delegate void LoadedEventHandler();

        public void Save(ConfigFile config)
        {
            config.SetValue(nameof(Player), nameof(Health), Health);

            WeaponManager.Save(config);
        }

        public void DeferredLoad(ConfigFile config)
        {
            if (config.HasSectionKey(nameof(Player), nameof(Health)))
            {
                int health = config.GetValue(nameof(Player), nameof(Health)).AsInt32();

                Heal(health - Health, true);
            }

            WeaponManager.Loaded += () => EmitSignal(SignalName.Loaded);

            WeaponManager.DeferredLoad(config);
        }

        #endregion

    }
}
