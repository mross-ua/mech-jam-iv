using Godot;
using System;
using MechJamIV;

public abstract partial class ProjectileBase : RigidBody2D,
    IProjectile
{

    public virtual void Prime()
    {
        // do nothing
    }

}
