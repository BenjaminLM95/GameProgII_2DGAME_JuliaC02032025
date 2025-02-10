using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class HealthSystem : Component
    {
        // ---------- REFERENCES ---------- //        
        GameObject _gameObject; // reference Player AND Enemy (as gameobjects?)

        // ---------- VARIABLES ---------- //
        // property Health
        public int Health { get; private set; } // reference past assignments from more detailed property        
        int maxHealth = 100; // variable maxHealth

        public int Damage { get; private set; } // property Damage       
        bool isAlive = true;

        // ---------- METHODS ---------- //
        private void TakeDamage(int damage)
        {
            if (Health <= 0)
            {
                Die();
            }
            Health -= damage;
        }

        private void Die()
        {
            // set gamaobject inactive (reference bool isActive from GameObject)
            _gameObject.IsActive = false; // ERROR: set acessor is inacessible
            // remove player sprite from dictionary?
        }
    }
}
