using Godot;
using System;

namespace MechJamIV.Interfaces
{
    public interface ICollidable
    {

        //[Signal]
        delegate void InjuredEventHandler(int damage);

        void Hurt(int damage, Vector2 globalPos, Vector2 normal);

    }
}
