using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public interface IWeapon
{

	//[Signal]
	public delegate void FiredEventHandler(int ammoRemaining);

}
