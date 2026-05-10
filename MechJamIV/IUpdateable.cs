using Godot;
using System;

namespace MechJamIV
{
    public interface IUpdateable<T>
    {

        void UpdateFrom(T source);

    }
}