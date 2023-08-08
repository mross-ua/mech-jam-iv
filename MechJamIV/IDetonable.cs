using Godot;
using System;

namespace MechJamIV
{
    public interface IDetonable
    {

        //[Signal]
        public delegate void DetonatedEventHandler();

        public float FuseDelay { get; }

        public void PrimeFuse();

    }
}
