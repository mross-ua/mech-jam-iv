using Godot;
using System;

namespace MechJamIV.Interfaces
{
    public interface IDetonable
    {

        //[Signal]
        delegate void DetonatedEventHandler();

        float FuseDelay { get; }

        void PrimeFuse();

    }
}
