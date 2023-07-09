using Godot;
using System;
using MechJamIV;

public partial class EnemyTroid : EnemyBase
{

    protected override Vector2 Gravity { get; set; } = Vector2.Zero;

	public override void _Ready()
	{
		base._Ready();
	}

	protected override Vector2 GetMovementDirection(double delta) => Vector2.Zero;

    protected override bool IsJumping() => false;

	protected override bool IsAttacking() => false;

    protected override void ProcessAttack(double delta)
    {
        throw new NotImplementedException();
    }

}
