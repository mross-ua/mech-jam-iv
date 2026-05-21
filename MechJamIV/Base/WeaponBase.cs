using Godot;
using MechJamIV.Enums;
using MechJamIV.Interfaces;
using System;
using System.Collections.Generic;

namespace MechJamIV.Base;

public abstract partial class WeaponBase : Node2D
    , IWeapon
{

    private bool isCoolingDown = false;

    public abstract void SetBodiesToExclude(IEnumerable<PhysicsBody2D>? bodies);

    protected abstract void FireSpecial(Vector2 globalPos, PhysicsBody2D? target = null);

    #region IWeapon

    [Signal]
    public delegate void FiredEventHandler();

    [Signal]
    public delegate void AmmoAddedEventHandler();

    public abstract PickupType WeaponType { get; }

    [Export]
    public float RoundsPerSecond { get; set; }

    [Export]
    public int Ammo { get; set; }

    [Export(PropertyHint.Layers2DPhysics)]
    public uint CollisionMask { get; set; }

    [Export]
    public float LineOfSightDistance { get; set; }

    public abstract Texture2D UISprite { get; }

    public async void Fire(Vector2 globalPos, PhysicsBody2D? target = null)
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

        Ammo--;

        isCoolingDown = true;

        FireSpecial(globalPos, target);

        EmitSignal(SignalName.Fired);

        if (!Mathf.IsZeroApprox(RoundsPerSecond))
        {
            await ToSignal(GetTree().CreateTimer(1.0f / RoundsPerSecond, false, true), SceneTreeTimer.SignalName.Timeout);
        }

        isCoolingDown = false;
    }

    public void AddAmmo(int count)
    {
        if (Ammo < 0)
        {
            // allow this for infinite ammo

            return;
        }

        SetAmmo(Ammo + count);
    }

    public void SetAmmo(int count)
    {
        if (Ammo < 0)
        {
            // allow this for infinite ammo

            return;
        }

        Ammo = Math.Clamp(count, 0, 99);

        EmitSignal(SignalName.AmmoAdded);
    }

    #endregion

}
