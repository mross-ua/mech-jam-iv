using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class Grenade : ExplosiveBarrel
{

	[Export]
	public override int Health { get; set; } = 1;

	public async void PullPinAsync()
	{
		await ToSignal(GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);

		HurtAsync(Health, Vector2.Zero);
	}

}
