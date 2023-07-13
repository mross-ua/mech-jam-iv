using Godot;
using System;
using MechJamIV;

public partial class GrenadePickup : PickupBase
{

    protected override PickupType PickupType => PickupType.Grenade;

	#region Node references

	private Grenade grenade;

	#endregion

	#region Resources

	private PackedScene shrapnelSplatter = ResourceLoader.Load<PackedScene>("res://scenes/shrapnel_splatter.tscn");

	#endregion

	public override void _Ready()
	{
		base._Ready();

		grenade = GetNode<Grenade>("Grenade");
		grenade.Killed += () => this.TimedFree(5.0f, processInPhysics: true);
	}

	public void Hurt(int damage, Vector2 position, Vector2 normal)
	{
		grenade.Hurt(damage, position, normal);
	}

}
