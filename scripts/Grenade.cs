using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class Grenade : ExplosiveBarrel
{

	[Export]
	public override int Health { get; set; } = 1;

	public async System.Threading.Tasks.Task PullPinAsync()
	{
		await ToSignal(GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);

		await HurtAsync(Health, Vector2.Zero);
	}

}
