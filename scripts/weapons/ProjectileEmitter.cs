using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class ProjectileEmitter : WeaponBase
{

    private PackedScene projectile = null;

    [Export]
    public PackedScene Projectile
    {
        get => projectile;
        set
        {
            if (value == null)
            {
                weaponType = (PickupType)(-1);
                uiSprite = null;
            }
            else
            {
                Projectile item = value.Instantiate<Projectile>();

                weaponType = item.WeaponType;
                uiSprite = item.UISprite;

                // we have to free it ourselves because we don't add it to the scene tree
                item.Free();
            }

            projectile = value;
        }
    }

    [Export]
    public float ImpulseStrength { get; set; }

    private IList<PhysicsBody2D> bodiesToExclude = null;

    public override void SetBodiesToExclude(IEnumerable<PhysicsBody2D> bodies)
    {
        bodiesToExclude = bodies == null ? null : new List<PhysicsBody2D>(bodies);
    }

    protected override async void _Fire(Vector2 globalPos, PhysicsBody2D target = null)
    {
        Projectile projectile = Projectile.Instantiate<Projectile>();
        projectile.GlobalTransform = GlobalTransform;

        projectile.SetBodiesToExclude(bodiesToExclude);

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
            if (target == null)
            {
                m.Rotate(m.CharacterAnimator.SpriteFaceDirection.AngleTo(dir));
            }

            m.CharacterTracker.Track(target);
        }
    }

    #region IWeapon

    private PickupType weaponType = (PickupType)(-1);
    private Texture2D uiSprite = null;

    public override PickupType WeaponType
    {
        get
        {
            if (!Enum.IsDefined(weaponType))
            {
                throw new InvalidOperationException("Projectile is not set");
            }

            return weaponType;
        }
    }

    public override Texture2D UISprite
    {
        get
        {
            if (uiSprite == null)
            {
                throw new InvalidOperationException("Projectile is not set");
            }

            return uiSprite;
        }
    }

    #endregion

}
