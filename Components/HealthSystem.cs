using System;
using System.Diagnostics;
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
        public void TakeDamage(int damage)
        {
            if (!isAlive) return;

            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isAlive = false;
                Die();
            }
            
            ShowDamageEffect(damage); // update health bar
        }

        private void Die()
        {
            //GameObject.IsActive = false;

            if (isPlayer)
            {
                Debug.WriteLine("Game Over!");
            }
        }

        private void ShowDamageEffect(int damage)
        {
            // floating text showing damage above sprite
            //DamageText damageText = new DamageText(damage, this.GameObject.Position);
            //Globals.spriteBatch.DrawString(damageText.Font, damageText.Text, damageText.Position, Color.Red);
        }
    }
}
