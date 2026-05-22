using Godot;
using System;

namespace MechJamIV.Interfaces;

public interface IPlayable : IDestructible
{

    //[Signal]
    delegate void ImmunityShieldActivatedEventHandler();
    //[Signal]
    delegate void ImmunityShieldDeactivatedEventHandler();

    void SetRemoteTarget(Camera2D cam);

}
