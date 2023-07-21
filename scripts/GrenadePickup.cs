using Godot;
using System;
using MechJamIV;

public partial class GrenadePickup : PickupBase
{

	public override PickupType PickupType { get; protected set;} = PickupType.Grenade;

	#region Node references

	private Grenade grenade;

	#endregion

	public override void _Ready()
	{
		base._Ready();

		grenade = GetNode<Grenade>("Grenade");
		grenade.Killed += () => this.TimedFree(5.0f, processInPhysics: true);
	}

	#region IDestructible

	public void Hurt(int damage, Vector2 globalPos, Vector2 normal)
	{
		grenade.Hurt(damage, globalPos, normal);
	}

	#endregion

}
