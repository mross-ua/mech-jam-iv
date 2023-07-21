using Godot;
using System;
using System.Collections.Generic;
using MechJamIV;

namespace MechJamIV {
	public interface IDestructible
	{

        //[Signal]
        public delegate void InjuredEventHandler(int damage);

        //[Signal]
        public delegate void KilledEventHandler();

        //[Signal]
        public delegate void HealedEventHandler(int health);

        public int Health { get; }

        public void Hurt(int damage, Vector2 globalPos, Vector2 normal);

		public void Heal(int health);

	}
}
