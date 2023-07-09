using Godot;
using System;
using MechJamIV;

public partial class EnemyMech : EnemyBase
{

	public override void _Ready()
	{
		base._Ready();
	}

	protected override Vector2 GetMovementDirection() => Vector2.Zero;

    protected override bool IsJumping() => false;

	protected override bool IsAttacking() => false;

    protected override void ProcessAttack(double delta)
    {
        throw new NotImplementedException();
    }

}
