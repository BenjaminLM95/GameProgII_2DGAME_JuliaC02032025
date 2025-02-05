using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class HealthSystem : Component
    {
        // reference Player AND Enemy (as gameobjects?)
        GameObject _gameObject;

        // property Health
        public int Health { get; private set; } // reference past assignments from more detailed property
        // variable maxHealth
        int maxHealth = 100;
        // property Damage
        public int Damage { get; private set; }
        // bool isAlive
        bool isAlive = true;

        // METHODS
        // TakeDamage()
        private void TakeDamage(int damage)
        {
            if (Health <= 0)
            {
                Die();
            }
            Health -= damage;
        }
        // Die()
        private void Die()
        {
            // set gamaobject inactive (reference bool isActive from GameObject)
            _gameObject.IsActive = false; // ERROR: set acessor is inacessible
        }
    }
}
