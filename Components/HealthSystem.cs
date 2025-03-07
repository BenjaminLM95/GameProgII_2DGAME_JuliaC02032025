using System;
using System.Numerics;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class HealthSystem : Component
    {
        Globals globals;

        // ---------- VARIABLES ---------- //
        // property Health
        public int Health { get; private set; } = 100; // reference past assignments from more detailed property
        public int currentHealth;
        int maxHealth = 100; // variable maxHealth
        public Vector2 tilePosition;
        public int Damage { get; private set; } // property Damage       
        bool isAlive = true;
        public bool isPlayer = false;

        // ---------- METHODS ---------- //

        public override void Start()
        {
            currentHealth = Health;
        }
        private void TakeDamage(int damage)
        {
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
                isAlive = false;
            }
            Health -= damage;
            // update health bar
        }

        private void Die()
        {
            // set gamaobject inactive (reference bool isActive from GameObject)
            
            if(isPlayer)
            {
                // show game over
            }
        }

        private void HealthBar()
        {
            // for every sprite, display a health bar below them. with damage, take away from float?
        }
    }
}
