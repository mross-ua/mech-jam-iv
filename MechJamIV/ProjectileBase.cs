using Godot;
using System;
using MechJamIV;

public abstract partial class ProjectileBase : RigidBody2D
    ,ICollidable
{

	#region ICollidable

    [Signal]
    public delegate void InjuredEventHandler(int damage);

	public abstract void Hurt(int damage, Vector2 globalPos, Vector2 normal);

	#endregion

}
