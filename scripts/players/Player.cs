using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MechJamIV;

public partial class Player : CharacterBase
    , IPlayable
{

    private bool isImmune = false;
    private bool isJumping = false;

    #region Node references

    public Marker2D RobotMarker { get; private set; }
    public WeaponManager WeaponManager { get; private set; }

    private GpuParticles2D immunityShield;
    private RemoteTransform2D remoteTransform;

    #endregion

    public override void _Ready()
    {
        base._Ready();

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

    protected override bool _IsJumping()
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

        WeaponManager.Fire(mode, globalPos, CharacterTracker.Target);
    }

    protected override void AnimateInjury(int damage, Vector2 globalPos, Vector2 normal)
    {
        this.EmitParticlesOnce(PointDamageEffect.Instantiate<GpuParticles2D>(), globalPos);
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

    public async override void Hurt(int damage, Vector2 globalPos, Vector2 normal)
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

}
