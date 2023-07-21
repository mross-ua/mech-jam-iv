using Godot;
using System;
using MechJamIV;

public partial class WeaponManager : Node2D
{

	[Export]
	public int GrenadeCount { get; set; } = 4;
	[Export]
	public float ThrowStrength { get; set; } = 500.0f;

	#region Node references

	private HitScanBulletEmitter hitScanBulletEmitter;

	#endregion

	#region Resources

	private PackedScene grenadeResource = ResourceLoader.Load<PackedScene>("res://scenes/weapons/grenade.tscn");

    #endregion

    public override void _Ready()
    {
		hitScanBulletEmitter = GetNode<HitScanBulletEmitter>("HitScanBulletEmitter");
    }

	public void Fire(FireMode mode, Vector2 globalPos)
	{
		switch (mode)
		{
			case FireMode.Primary:
				hitScanBulletEmitter.Fire(globalPos);

				break;
			case FireMode.PrimarySustained:
				hitScanBulletEmitter.Fire(globalPos);

				break;
			case FireMode.Secondary:
				ThrowGrenade(globalPos);

				break;
		}
	}

	private async void ThrowGrenade(Vector2 globalPos)
	{
		if (GrenadeCount <= 0)
		{
			return;
		}

		GrenadeCount--;

		Grenade grenade = grenadeResource.Instantiate<Grenade>();
		grenade.GlobalTransform = hitScanBulletEmitter.GlobalTransform;

		await GetTree().CurrentScene.AddChildDeferred(grenade);

		grenade.Prime();

		grenade.ApplyImpulse((globalPos - hitScanBulletEmitter.GlobalTransform.Origin).Normalized() * ThrowStrength);
	}

}
