using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

public partial class Grenade : ExplosiveBarrel
{

    public override Texture2D SpriteTexture => GetNode<CharacterAnimator>("CharacterAnimator").SpriteFrames.GetFrameTexture("idle", 0);

}
