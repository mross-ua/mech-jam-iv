using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

namespace MechJamIV
{
    public interface ICollidable
    {

        //[Signal]
        public delegate void InjuredEventHandler(int damage);

        public void Hurt(int damage, Vector2 globalPos, Vector2 normal);

    }
}
