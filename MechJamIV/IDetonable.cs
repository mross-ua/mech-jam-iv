using Godot;
using System;

namespace MechJamIV
{
    public interface IDetonable
    {

        //[Signal]
        delegate void DetonatedEventHandler();

        float FuseDelay { get; }

        void PrimeFuse();

    }
}
