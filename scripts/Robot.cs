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

    protected override void ProcessAction()
	{
		//TODO
	}

	protected override void AnimateInjury(int damage, Vector2 position, Vector2 normal)
	{
		//TODO
	}

	#region ICollidable

    public override void Hurt(int damage, Vector2 globalPos, Vector2 normal)
    {
		// ignore damage
        //base.Hurt(damage, globalPos, normal);
    }

	#endregion

}
