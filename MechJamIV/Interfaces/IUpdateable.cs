using Godot;
using System;

namespace MechJamIV.Interfaces;

public interface IUpdateable
{

    //[Signal]
    delegate void LoadedEventHandler();

    void Save(ConfigFile config);

    void DeferredLoad(ConfigFile config);

}
