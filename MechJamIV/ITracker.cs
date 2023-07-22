using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

namespace MechJamIV {
	public interface ITracker
	{

		// from Node2D
		public World2D GetWorld2D();
        public Transform2D GlobalTransform { get; }

		public CollisionLayerMask LineOfSightMask { get; }

		public CharacterBase Target { get; }

		public void Track(CharacterBase target, CollisionLayerMask lineOfSightMask);

	}
}
