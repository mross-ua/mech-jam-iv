using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MechJamIV;

public partial class HitScanBulletEmitter : WeaponBase
{

    [Export]
    public PackedScene PointDamageEffect { get; set; }

    [Export]
    public int Damage { get; set; }

    [Export(PropertyHint.ColorNoAlpha)]
    public Color TracerColor { get; set; }

    [Export]
    public float TracerWidth { get; set; }

    private readonly Queue<Tuple<Vector2, Vector2>> bulletsToDraw = new();

    private Godot.Collections.Array<Rid> bodiesToExclude = null;

    private bool isNeedsRedraw = false;

    public override void _Process(double delta)
    {
        if (isNeedsRedraw)
        {
            QueueRedraw();
        }
    }

    public override void _Draw()
    {
        isNeedsRedraw = false;

        while (bulletsToDraw.TryDequeue(out Tuple<Vector2, Vector2> rayPath))
        {
            DrawLine(ToLocal(rayPath.Item1), ToLocal(rayPath.Item2), TracerColor, TracerWidth);

            // we need to draw at least one more frame to *clear* anything drawn this frame
            isNeedsRedraw = true;
        }
    }

    public override void SetBodiesToExclude(IEnumerable<PhysicsBody2D> bodies)
    {
        if (bodies == null)
        {
            bodiesToExclude = null;
        }
        else
        {
            bodiesToExclude = new Godot.Collections.Array<Rid>(bodies.Select(b => b.GetRid()));
        }
    }

    protected override void _Fire(Vector2 globalPos, PhysicsBody2D target = null)
    {
        Vector2 from = GlobalPosition;
        Vector2 to = from + from.DirectionTo(globalPos) * LineOfSightDistance;

        Godot.Collections.Dictionary collision = GetWorld2D().DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters2D()
        {
            From = from,
            To = to,
            Exclude = bodiesToExclude,
            CollideWithBodies = true,
            CollideWithAreas = true,
            CollisionMask = CollisionMask
        });

        if (collision.ContainsKey("collider"))
        {
            Vector2 position = collision["position"].AsVector2();
            Vector2 normal = collision["normal"].AsVector2();

            if (collision["collider"].Obj is Hitbox hitbox)
            {
                hitbox.Hurt(Damage, position, normal);
            }
            else if (collision["collider"].Obj is ICollidable c)
            {
                c.Hurt(Damage, position, normal);
            }
            else
            {
                // world or environment hit

                this.EmitParticlesOnce(PointDamageEffect.Instantiate<GpuParticles2D>(), position);
            }

            bulletsToDraw.Enqueue(new Tuple<Vector2, Vector2>(from, position));
        }
        else
        {
            bulletsToDraw.Enqueue(new Tuple<Vector2, Vector2>(from, to));
        }

        isNeedsRedraw = true;
    }

    #region IWeapon

    public override PickupType WeaponType => PickupType.Rifle;

    public override Texture2D UISprite
    {
        get
        {
            // NOTE: This must be accessible outside of scene tree.
            //       (_Ready() may not have been called.)

            return GetNode<Sprite2D>("UISprite").Texture;
        }
    }

    #endregion

}
