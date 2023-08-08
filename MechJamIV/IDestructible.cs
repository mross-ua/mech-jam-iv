using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

namespace MechJamIV {
	public interface IDestructible : ICollidable
	{

        //[Signal]
        public delegate void KilledEventHandler();

        //[Signal]
        public delegate void HealedEventHandler(int health);

        public int MaxHealth { get; }

        public int MaxOverHealth { get; }

        public int Health { get; }

		public void Heal(int health, bool allowOverHealth);

	}
}
