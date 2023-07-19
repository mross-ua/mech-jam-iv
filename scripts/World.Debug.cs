#if DEBUG

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using MechJamIV;

public partial class World : Node2D
{

    public override void _Draw()
    {
		DrawLine(player.HitScanBulletEmitter.GlobalTransform.Origin, playerCamera.GetGlobalMousePosition(), Colors.Green, 1.0f);
    }

}

#endif