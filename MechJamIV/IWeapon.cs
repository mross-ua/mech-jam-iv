using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public interface IWeapon
{

	//[Signal]
	public delegate void FiredEventHandler();

	public float RoundsPerSecond { get; }

	public int Ammo { get; }

	//[Export(PropertyHint.Layers2DPhysics)]
	public uint LineOfSightMask { get; set; }

	public float LineOfSightDistance { get; set; }

	public void Fire(Vector2 globalPos, CharacterBase target = null);

}
