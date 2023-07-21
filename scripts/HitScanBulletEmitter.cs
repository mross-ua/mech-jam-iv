using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MechJamIV;

public partial class HitScanBulletEmitter : Node2D
{

	[Export]
	public float HitScanDistance { get; set; } = 10_000.0f;
	[Export]
	public int Damage { get; set; } = 1;
	[Export(PropertyHint.ColorNoAlpha)]
	public Color TracerColor { get; set; } = Colors.LightYellow;

	private Queue<Tuple<Vector2, Vector2>> bulletsToDraw = new Queue<Tuple<Vector2, Vector2>>();

	private Godot.Collections.Array<Rid> bodiesToExclude = null;

	private bool isNeedsRedraw = false;

	#region Node references

	private Timer attackTimer;

	#endregion

	#region Resources

	private static readonly PackedScene shrapnelSplatter = ResourceLoader.Load<PackedScene>("res://scenes/effects/shrapnel_splatter.tscn");

    #endregion

    public override void _Ready()
    {
		attackTimer = GetNode<Timer>("AttackTimer");
    }

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
			DrawLine(ToLocal(rayPath.Item1), ToLocal(rayPath.Item2), TracerColor, 1.0f);

			// we need to draw at least one more frame to *clear* anything drawn this frame
			isNeedsRedraw = true;
		}
	}

	public void SetBodiesToExclude(IEnumerable<Rid> resourceIds)
	{
		bodiesToExclude = new Godot.Collections.Array<Rid>(resourceIds);
	}

	public void Fire(Vector2 globalPos)
	{
		if (attackTimer.TimeLeft > 0)
		{
			return;
		}

		attackTimer.Start();

		Vector2 from = GlobalTransform.Origin;
		Vector2 to = globalPos + (globalPos - GlobalTransform.Origin).Normalized() * HitScanDistance;

		Godot.Collections.Dictionary collision = GetWorld2D().DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters2D()
		{
			From = from,
			To = to,
			Exclude = bodiesToExclude,
			CollideWithBodies = true,
			CollideWithAreas = true,
			CollisionMask = (uint)(CollisionLayerMask.World | CollisionLayerMask.Environment | CollisionLayerMask.Hitbox)
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
				c.Hurt(Damage, position,  normal);
			}
			else
			{
				// world or environment hit

        		this.EmitParticlesOnce(shrapnelSplatter.Instantiate<GpuParticles2D>(), globalPos);
			}

			bulletsToDraw.Enqueue(new Tuple<Vector2, Vector2>(from, position));
		}
		else
		{
			bulletsToDraw.Enqueue(new Tuple<Vector2, Vector2>(from, to));
		}

		isNeedsRedraw = true;
	}

}
