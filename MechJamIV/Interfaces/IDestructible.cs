using Godot;
using System;

namespace MechJamIV.Interfaces;

public interface IDestructible : ICollidable
{

    //[Signal]
    delegate void KilledEventHandler();

    //[Signal]
    delegate void HealedEventHandler(int health);

    int MaxHealth { get; }

    int MaxOverHealth { get; }

    int Health { get; }

    void Heal(int health, bool allowOverHealth);

}
