#if DEBUG

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MechJamIV {
    public abstract partial class CharacterBase
    {

        public override void _Draw()
        {
            DrawLine(Vector2.Zero, GetMovementDirection() * 25, Colors.Red);
        }

    }
}

#endif
