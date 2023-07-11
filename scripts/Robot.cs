using Godot;
using System;
using MechJamIV;

public partial class Robot : CharacterBase
{


	#region Node references

	private Player player;

	#endregion

    public override void _Ready()
    {
		base._Ready();

		player = (Player)GetTree().GetFirstNodeInGroup("player");
    }

	protected override Vector2 GetMovementDirection() => GlobalTransform.Origin.DirectionTo(player.RobotMarker.GlobalTransform.Origin);

    protected override bool IsJumping() => false;

	protected override bool IsAttacking() => false;

    protected override void ProcessAttack(double delta)
	{
		//TODO
	}

	protected async override void AnimateInjuryAsync(int damage, Vector2 normal)
	{
		//TODO
	}

}
