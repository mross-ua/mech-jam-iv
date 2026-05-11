using Godot;
using System;

namespace MechJamIV
{
    public interface IUpdateable<T>
    {

        //[Signal]
        delegate void UpdatedEventHandler();

        void DeferredUpdateFrom(T source);

    }
}