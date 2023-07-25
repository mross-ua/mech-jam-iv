using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MechJamIV;

public partial class HitScanBulletEmitter : Node2D
{

	[Export(PropertyHint.Layers2DPhysics)]
	public uint LineOfSightMask { get; set; }
	[Export]
	public float LineOfSightDistance { get; set; }
	[Export]
	public int Damage { get; set; }
	[Export]
	public float RoundsPerSecond { get; set; }
	[Export(PropertyHint.ColorNoAlpha)]
	public Color TracerColor { get; set; }
	[Export]
	public float TracerWidth { get; set; }

	private Queue<Tuple<Vector2, Vector2>> bulletsToDraw = new Queue<Tuple<Vector2, Vector2>>();

	private Godot.Collections.Array<Rid> bodiesToExclude = null;

	private bool isCoolingDown = false;

	private bool isNeedsRedraw = false;

	#region Resources

	private static readonly PackedScene shrapnelSplatter = ResourceLoader.Load<PackedScene>("res://scenes/effects/shrapnel_splatter.tscn");

    #endregion

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

	public void SetBodiesToExclude(IEnumerable<PhysicsBody2D> bodies)
	{
		bodiesToExclude = new Godot.Collections.Array<Rid>(bodies.Select(b => b.GetRid()));
	}

	public async void Fire(Vector2 globalPos)
	{
		if (isCoolingDown)
		{
			return;
		}

		isCoolingDown = true;

		Vector2 from = GlobalTransform.Origin;
		Vector2 to = GlobalTransform.Origin + (globalPos - GlobalTransform.Origin).Normalized() * LineOfSightDistance;

		Godot.Collections.Dictionary collision = GetWorld2D().DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters2D()
		{
			From = from,
			To = to,
			Exclude = bodiesToExclude,
			CollideWithBodies = true,
			CollideWithAreas = true,
			CollisionMask = LineOfSightMask
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

        		this.EmitParticlesOnce(shrapnelSplatter.Instantiate<GpuParticles2D>(), position);
			}

			bulletsToDraw.Enqueue(new Tuple<Vector2, Vector2>(from, position));
		}
		else
		{
			bulletsToDraw.Enqueue(new Tuple<Vector2, Vector2>(from, to));
		}

		isNeedsRedraw = true;

		if (!Mathf.IsZeroApprox(RoundsPerSecond))
		{
			await ToSignal(GetTree().CreateTimer(1.0f / RoundsPerSecond, false, true), SceneTreeTimer.SignalName.Timeout);
		}

		isCoolingDown = false;
	}

}
