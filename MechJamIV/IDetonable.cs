using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

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
