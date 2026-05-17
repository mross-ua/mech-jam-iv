using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;
using System.Diagnostics;

public partial class ProjectileEmitter : WeaponBase
{

    private PackedScene projectile = null!;

    [Export]
    public PackedScene Projectile
    {
        get => projectile;
        set
        {
            projectile = value;

            Projectile item = projectile.Instantiate<Projectile>();

            weaponType = item.WeaponType;
            uiSprite = item.UISprite;

            // we have to free it ourselves because we don't add it to the scene tree
            item.Free();
        }
    }

    [Export]
    public float ImpulseStrength { get; set; }

    private IEnumerable<PhysicsBody2D>? bodiesToExclude = null;

    public override void _Ready()
    {
        Debug.Assert(Projectile is not null, $"{nameof(Projectile)} must not be null");
    }

    public override void SetBodiesToExclude(IEnumerable<PhysicsBody2D>? bodies)
    {
        bodiesToExclude = bodies;
    }

    protected override async void FireSpecial(Vector2 globalPos, PhysicsBody2D? target = null)
    {
        Projectile projectile = Projectile.Instantiate<Projectile>();
        projectile.GlobalTransform = GlobalTransform;

        projectile.SetBodiesToExclude(bodiesToExclude);

        // this adds to the top-level scene!
        await GetTree().CurrentScene.AddChildDeferred(projectile);

        Vector2 dir = (globalPos - GlobalPosition).Normalized();

        projectile.ApplyImpulse(dir * ImpulseStrength);

        if (projectile is IDetonable d)
        {
            d.PrimeFuse();
        }

        //TODO we need another way to identify trackers (since we got rid of ITracker)
        if (projectile is Missile m)
        {
            if (target is null)
            {
                m.Rotate(m.CharacterAnimator.SpriteFaceDirection.AngleTo(dir));
            }

            m.CharacterTracker!.Track(target);
        }
    }

    #region IWeapon

    private PickupType weaponType = (PickupType)(-1);
    private Texture2D uiSprite = null!;

    public override PickupType WeaponType => weaponType;

    public override Texture2D UISprite => uiSprite;

    #endregion

}
