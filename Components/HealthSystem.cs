using System;
using System.Diagnostics;
using System.Numerics;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal class HealthSystem : Component
    {
        Globals globals;

        // ---------- VARIABLES ---------- //
        // property Health
        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; private set; } = 100;
        public enum EntityType
        {
            Player,
            Enemy,
            Other
        }
        public EntityType Type { get; set; } = EntityType.Other;

        public bool IsAlive { get; private set; } = true; // State tracking

        public Action OnDeath { get; set; } // custom death handling

        // Constructor with optional max health and entity type
        public HealthSystem(int maxHealth = 100, EntityType type = EntityType.Other)
        {
            MaxHealth = maxHealth;
            Type = type;
        }

        // --------------------------//
        public Vector2 tilePosition;
        public int Damage { get; private set; } // property Damage       
        public bool isPlayer = false;

        // ---------- METHODS ---------- //

        public override void Start()
        {
            CurrentHealth = MaxHealth; // Initialize health to full
            IsAlive = true;
        }
        public void TakeDamage(int damage)
        {
            if (!IsAlive) return;

            CurrentHealth -= damage;

            // Show damage effect (can be customized)
            ShowDamageEffect(damage);

            // Check if entity dies
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                IsAlive = false;
                Die();
            }
        }
        public void ModifyHealth(int amount)
        {
            if (!IsAlive) return;

            CurrentHealth = Math.Clamp(CurrentHealth + amount, 0, MaxHealth);

            // Optional: trigger any health change events
            if (amount > 0)
            {
                Debug.WriteLine($"{Type} healed for {amount}. Current Health: {CurrentHealth}");
            }
            else if (amount < 0)
            {
                Debug.WriteLine($"{Type} damaged for {Math.Abs(amount)}. Current Health: {CurrentHealth}");
            }
        }
        private void Die()
        {
            switch (Type)
            {
                case EntityType.Player:
                    HandlePlayerDeath();
                    break;
                case EntityType.Enemy:
                    HandleEnemyDeath();
                    break;
            }

            // Invoke custom death callback if set
            OnDeath?.Invoke();
        }
        private void HandlePlayerDeath()
        {
            Debug.WriteLine("Game Over!");
            // player-specific death logic
            // game over screen, reset level
        }

        private void HandleEnemyDeath()
        {
            Debug.WriteLine("Enemy defeated!");
            // enemy-specific death logic
            // drop items, grant experience
        }

        private void ShowDamageEffect(int damage)
        {
            // show floating text
            Debug.WriteLine($"{Type} took {damage} damage");
        }

        public void Revive() // revive/reset to full health
        {
            CurrentHealth = MaxHealth;
            IsAlive = true;
        }
    }
}
