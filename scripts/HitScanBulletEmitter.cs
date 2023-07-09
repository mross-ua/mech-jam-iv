using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MechJamIV;

public partial class HitScanBulletEmitter : Node2D
{

	[Export]
	public float HitScanDistance { get; set; } = 10000.0f;

	private Godot.Collections.Array<Rid> _bodiesToExclude = new ();

	public int Damage { get; set; } = 1;

	private Vector2 gravity = ProjectSettings.GetSetting("physics/2d/default_gravity_vector").AsVector2().Normalized() * ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	//private PackedScene bulletHitEffect = ResourceLoader.Load<PackedScene>("res://assets/effects/bullet_hit_effect.tscn");

	#region Node references

	//TODO remove! this is temporary!
	private Camera2D camera2D;

	#endregion

	public override void _Ready()
	{
		camera2D = GetParent().GetNode<Camera2D>("Camera2D");
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	private Vector2 GetRelativeMousePosition()
	{
		//return GetViewport().GetMousePosition();
		//return GetGlobalMousePosition();
		return camera2D.GetGlobalMousePosition() - GlobalTransform.Origin;
	}

    public override void _Draw()
    {
        base._Draw();

		DrawLine(Vector2.Zero, GetRelativeMousePosition(), Colors.Green, 1.0f);
    }

	public void SetBodiesToExclude(IEnumerable<Rid> resourceIds)
	{
		_bodiesToExclude = new Godot.Collections.Array<Rid>(resourceIds);
	}

	public void Fire()
	{
		Vector2 mousePos = GetRelativeMousePosition();

		Godot.Collections.Dictionary collision = GetWorld2D().DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters2D()
		{
			From = GlobalTransform.Origin,
			//TODO why does this work?
			To = mousePos.Normalized() * HitScanDistance,
			Exclude = _bodiesToExclude,
			CollideWithBodies = true,
			CollideWithAreas = true,
			CollisionMask = (uint)(CollisionLayerMask.Environment | CollisionLayerMask.Hitbox)
		});

		if (collision.ContainsKey("collider"))
		{
			Vector2 normal = collision["normal"].AsVector2();

			if (collision["collider"].Obj is Hitbox hitbox)
			{
				hitbox.Hurt(Damage, normal);
			}
			else
			{
				// environment hit

				// BulletHitEffect node = bulletHitEffect.Instantiate<BulletHitEffect>();

				// GetTree().Root.AddChild(node);

				// node.GlobalPosition = collision["position"].AsVector3();

				// if (Mathf.IsZeroApprox(normal.AngleTo(-gravity)))
				// {
				// 	//TODO why do we return here? this causes ground shots to not have the effect
				// 	return;
				// }
				// else if (Mathf.IsZeroApprox(normal.AngleTo(gravity)))
				// {
				// 	node.Rotate(Vector3.Right, Mathf.Pi);

				// 	//TODO why do we return here? presumably this would cause ceiling shots to not have an effect
				// 	return;
				// }

				// Vector3 y = normal;
				// Vector3 x = y.Cross(-gravity);
				// Vector3 z = x.Cross(y);

				// node.Basis = new Basis(x, y, z);

				// //TODO remove node after effect is over

				// node.EmitOnce();
			}
		}
	}

}
