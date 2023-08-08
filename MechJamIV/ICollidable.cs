using Godot;
using System;

namespace MechJamIV
{
    public interface ICollidable
    {

        //[Signal]
        public delegate void InjuredEventHandler(int damage);

        public void Hurt(int damage, Vector2 globalPos, Vector2 normal);

    }
}
