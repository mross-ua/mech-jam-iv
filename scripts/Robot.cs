using Godot;
using System;
using MechJamIV;

public partial class Robot : CharacterBase
	,ITracker
{

    protected override Vector2 Gravity { get; set; } = Vector2.Zero;

	protected override Vector2 GetMovementDirection()
	{
		if (Target == null)
		{
			return Vector2.Zero;
		}

		//TODO there's a hard cast here--need to refactor...something.
		//     we don't need to rely on the marker--the robot will eventually
		//     have other logic or user input
		return GlobalTransform.Origin.DirectionTo(((Player)Target).RobotMarker.GlobalTransform.Origin);
	}

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

	#region ITracker

	public CollisionLayerMask LineOfSightMask { get => CollisionLayerMask.World | CollisionLayerMask.Player; }

	public CharacterBase Target { get; private set; }

	public void Track(CharacterBase c)
	{
		Target = c;

		Target.Killed += () => Untrack(c);
		// just in case we miss the Killed signal
		Target.TreeExiting += () => Untrack(c);
	}

	private void Untrack(CharacterBase c)
	{
		// make sure we are still tracking the object that fired this event
		if (Target == c)
		{
			Target = null;
		}
	}

	#endregion

}
