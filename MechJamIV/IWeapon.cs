using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

namespace MechJamIV {
	public interface IWeapon
	{

		//[Signal]
		public delegate void FiredEventHandler();

		public float RoundsPerSecond { get; }

		public int Ammo { get; }

		//[Export(PropertyHint.Layers2DPhysics)]
		public uint CollisionMask { get; }

		public float LineOfSightDistance { get; }

		public Texture2D SpriteTexture { get; }

		public void Fire(Vector2 globalPos, CollisionObject2D target = null);

	}
}