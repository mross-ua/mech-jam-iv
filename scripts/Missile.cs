using MechJamIV;

public partial class Missile : Grenade
{
    [Export]
    public Vector2 FaceDirection { get; set; } = Vector2.Up;

    [Export]
    public float ThrustForce { get; set; } = 5_000.0f;

    [Export]
    public float TurnSpeed { get; set; } = 3_000f;

    #region Node references

    protected Player Player { get; private set; }

    private GpuParticles2D gpuParticles2D;

    #endregion Node references

    public override void _Ready()
    {
        base._Ready();

        Player = (Player)GetTree().GetFirstNodeInGroup("player");

        gpuParticles2D = GetNode<GpuParticles2D>("GPUParticles2D");

        BodyEntered += (body) => Hurt(Health, GlobalTransform.Origin, Vector2.Zero);

#if DEBUG
        AddRayCastToPlayer();
#endif
    }

    public override void _Process(double delta)
    {
        if (Health <= 0)
        {
            return;
        }

        AnimateMovement();

#if DEBUG
        QueueRedraw();
#endif
    }

    private Vector2 GetMovementDirection(double delta)
    {
        Vector2 directionToPlayer = GetDirectionToPlayer();

        float angleDiff = Mathf.RadToDeg(FaceDirection.AngleTo(directionToPlayer));
        int turnDirection = Mathf.Sign(angleDiff);

        float rotation = TurnSpeed * (float)delta;

        if (Mathf.Abs(angleDiff) < rotation)
        {
            return directionToPlayer;
        }
        else
        {
            return FaceDirection.Rotated(Mathf.DegToRad(rotation) * turnDirection);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Health <= 0)
        {
            return;
        }

        FaceDirection = GetMovementDirection(delta);

        ApplyForce(FaceDirection * ThrustForce * (float)delta);

#if DEBUG
        UpdateRayCastToPlayer();
#endif
    }

    protected void AnimateMovement()
    {
        // NOTE: Rotating the graphics is a hack because we are using
        //       FaceDirection rather than a built-in property.

        CharacterAnimator.Rotation = Vector2.Up.AngleTo(FaceDirection);
        gpuParticles2D.Rotation = Vector2.Up.AngleTo(FaceDirection);
    }

    protected override void AnimateDeath()
    {
        base.AnimateDeath();

        gpuParticles2D.Visible = false;
    }

    protected Vector2 GetDirectionToPlayer() =>
        GlobalTransform.Origin.DirectionTo(Player.GlobalTransform.Origin);

    // protected bool IsPlayerInFieldOfView()
    // {
    // 	return Mathf.RadToDeg(FaceDirection.AngleTo(GetDirectionToPlayer())) < FieldOfView;
    // }

    protected bool IsPlayerInLineOfSight()
    {
        Godot.Collections.Dictionary collision = GetWorld2D().DirectSpaceState.IntersectRay(new PhysicsRayQueryParameters2D()
        {
			//TODO do we need to use Y basis rather than Vector2.Up?
			From = GlobalTransform.Origin + Vector2.Up, // offset so we don't collide with ground
			To = Player.GlobalTransform.Origin,
            Exclude = null,
            CollideWithBodies = true,
            CollideWithAreas = true,
            CollisionMask = (uint)(CollisionLayerMask.World | CollisionLayerMask.Player)
        });

        return collision.ContainsKey("collider") && collision["collider"].Obj == Player;
    }
}