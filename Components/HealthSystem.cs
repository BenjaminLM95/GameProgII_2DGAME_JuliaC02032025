using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Linq;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal class HealthSystem : Component
    {
        Globals globals;
        Combat combat;

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
        public bool IsDead { get; private set; } = false; // State tracking

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
            globals = Globals.Instance;

            if (globals == null)
            {
                Debug.WriteLine($"HealthSystem: Globals was NULL.");
            }
            combat = GameObject.GetComponent<Combat>();
            if (combat == null)
            {
                Debug.WriteLine("Inventory: Combat was NULL.");
            }

            CurrentHealth = MaxHealth; // Initialize health to full
            IsAlive = true;
        }
        public override void Update(float deltaTime)
        {
            if (combat == null) // if created later, iterate until it is found
            {
                combat = globals._combat;
                if (combat == null) return;
            }
        }
        public void TakeDamage(int damage)
        {
            if (!IsAlive) return;

            CurrentHealth -= damage;

            // Show damage effect (can be customized)
            //combat.DrawDamageIndicator(damage); // fix null combat reference, change dmg ind to GameHUD?
            Debug.WriteLine($"[HealthSystem] {Type} took {damage} damage. Current Health: {CurrentHealth}");

            // Check if entity dies
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                IsAlive = false;
                Die();
            }
        }

        // updates health when healed or damaged (when calling (-number) means damage)
        public void ModifyHealth(int amount)
        {
            if (!IsAlive) return;

            //CurrentHealth = Math.Clamp(CurrentHealth + amount, 0, MaxHealth);

            if (amount > 0)
            {
                Debug.WriteLine($"{Type} healed for {amount}. Current Health: {CurrentHealth}");
                CurrentHealth = Math.Clamp(CurrentHealth + amount, 0, MaxHealth);
            }
            else if (amount < 0)
            {
                TakeDamage(-amount);
            }
        }
        private void Die()
        {
            Debug.WriteLine($"[HealthSystem] {Type}'s health is 0. {Type} died!");

            if (Type == EntityType.Enemy)
            {
                // Try to find the Enemy component from the GameObject
                Enemy enemyComponent = GameObject.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    // Remove from the _enemies list
                    bool removed = Enemy._enemies.Remove(enemyComponent);
                    if (removed)
                    {
                        Debug.WriteLine($"HealthSystem: Enemy removed from _enemies list at position {GameObject.Position}");
                    }
                    else
                    {
                        Debug.WriteLine($"HealthSystem: Enemy could not be removed from _enemies (not found?)");
                    }

                    // Remove the GameObject from the scene
                    Globals.Instance._scene.RemoveGameObject(GameObject);
                }
                else
                {
                    Debug.WriteLine("HealthSystem: Enemy component not found on GameObject.");
                }
            }

            switch (Type)
            {
                case EntityType.Player:
                    HandlePlayerDeath();
                    break;
                case EntityType.Enemy:
                    EnemyDeath();
                    break;
            }

            // Invoke custom death callback if set
            OnDeath?.Invoke();
        }
        private void HandlePlayerDeath()
        {
            Debug.WriteLine("HealthSystem: player died, Game Over! :(");
            IsAlive = false;
            // player-specific death logic
            // game over screen, reset level
        }

        private void EnemyDeath()
        {
            if (Enemy.AllEnemies.Contains(GameObject))
            {
                Enemy.AllEnemies.Remove(GameObject);
                Globals.Instance._scene?.RemoveGameObject(GameObject);

                Debug.WriteLine($"HealthSystem: Enemy removed from the game at position {GameObject.Position}");
            }
            else
            {
                Debug.WriteLine("HealthSystem: Enemy not found in enemy list.");
            }
        }
        private void HandleEnemyDeath()
        {
            Debug.WriteLine("HealthSystem: Enemy defeated!");
            IsAlive = false;
            // enemy-specific death logic
            Enemy enemyObj = GameObject.FindObjectOfType<Enemy>();

            if (enemyObj != null)
            {
                // Get the list of enemies
                List<Enemy> enemies = enemyObj.GetEnemies();

                // Find the specific enemy with this HealthSystem component
                Enemy enemyToRemove = enemies.FirstOrDefault(e => e.GameObject == this.GameObject);

                if (enemyToRemove != null)
                {
                    enemies.Remove(enemyToRemove); // Remove the enemy from the list

                    // Remove the enemy GameObject from the scene
                    Globals.Instance._scene?.RemoveGameObject(enemyToRemove.GameObject);

                    Debug.WriteLine($"HealthSystem: Enemy removed from the game at position {enemyToRemove.GameObject.Position}");
                }
                else {
                    Debug.WriteLine("HealthSystem: Could not find enemy to remove");
                }
            }
            else {
                Debug.WriteLine("HealthSystem: Enemy manager not found");
            }
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
