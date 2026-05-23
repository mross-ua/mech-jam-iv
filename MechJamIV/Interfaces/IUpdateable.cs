using Godot;
using System;

namespace MechJamIV.Interfaces;

public interface IUpdateable
{

    void Save(ConfigFile config);

    void DeferredLoad(ConfigFile config);

}
