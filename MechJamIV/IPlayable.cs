using Godot;
using System;

namespace MechJamIV
{
    public interface IPlayable : IDestructible
    {

        //[Signal]
        public delegate void ImmunityShieldActivatedEventHandler();
        //[Signal]
        public delegate void ImmunityShieldDeactivatedEventHandler();

        public void SetRemoteTarget(Camera2D cam);

    }
}
