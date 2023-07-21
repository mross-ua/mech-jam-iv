using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

namespace MechJamIV {
	public interface IDetonatable
	{

        //[Signal]
        public delegate void DetonatedEventHandler();

    	public float FuseDelay { get; set; }

        public void PrimeFuse();

	}
}
