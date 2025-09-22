using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal class HealthSystem : Component
    {
        Globals globals;
        GameHUD hud;
        SpriteFont myFont; // FONT

        // ---------- VARIABLES ---------- //
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

        private List<FloatingDamageText> floatingTexts = new List<FloatingDamageText>();

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

            hud = GameObject.GetComponent<GameHUD>();
            myFont = Globals.content.Load<SpriteFont>("Minecraft"); // loading font

            CurrentHealth = MaxHealth; // Initialize health to full
            IsAlive = true;
        }
        public override void Update(float deltaTime)
        {
            if (hud == null)
                hud = GameObject.FindObjectOfType<GameHUD>();

            for (int i = floatingTexts.Count - 1; i >= 0; i--)
            {
                floatingTexts[i].Timer -= deltaTime;
                floatingTexts[i].Position.Y -= 30f * deltaTime; // float up

                if (floatingTexts[i].Timer <= 0)
                    floatingTexts.RemoveAt(i);
            }

            base.Update(deltaTime);
        }
        public void TakeDamage(int damage)
        {
            if (!IsAlive) return;

            CurrentHealth -= damage;

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
                Vector2 screenPosition = GameObject.Position;
                floatingTexts.Add(new FloatingDamageText(-amount, screenPosition));
            }
        }
        private void Die()
        {
            Debug.WriteLine($"[HealthSystem] {Type}'s health is 0. {Type} died!");

            if (Type == EntityType.Player)
            {
                HandlePlayerDeath();
            }
            else if (Type == EntityType.Enemy)
            {
                HandleEnemyDeath();
            }
            

            // Invoke custom death callback if set
            OnDeath?.Invoke();
        }
        public void HandlePlayerDeath()
        {
            Debug.WriteLine("HealthSystem: player died, Game Over! :(");
            IsAlive = false;
            // game over screen, reset level
            hud.isGameOverMenu = true;
        }

        private void HandleEnemyDeath()
        {
            Debug.WriteLine("HealthSystem: Enemy defeated!");
            IsAlive = false;

            Enemy enemyComponent = GameObject.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                // Boss Check
                if (enemyComponent.Type == EnemyType.Boss)
                {
                    Debug.WriteLine("HealthSystem: Boss defeated! Show win screen.");
                    if (hud != null)
                    {
                        //hud.isWinMenu = true; // win screen
                    }
                }

                if (Enemy._enemies.Remove(enemyComponent))
                {
                    Debug.WriteLine("HealthSystem: Removed from Enemy._enemies.");
                }

                if (Enemy.AllEnemies.Remove(GameObject))
                {
                    Debug.WriteLine("HealthSystem: Removed from Enemy.AllEnemies.");
                }
            }

            // Remove from TurnManager turn takers
            var turnManager = Globals.Instance._turnManager;
            if (turnManager != null && turnManager.turnTakers.Contains(enemyComponent))
            {
                turnManager.turnTakers.Remove(enemyComponent);
                Debug.WriteLine("HealthSystem: Removed from TurnManager.TurnTakers.");
            }

            // Remove from scene
            Globals.Instance._scene?.RemoveGameObject(GameObject);
            Debug.WriteLine($"HealthSystem: GameObject destroyed at {GameObject.Position}");
        }
        public void ResetHealth()
        {
            CurrentHealth = 100;
            IsAlive = true;
            Debug.WriteLine("HealthSystem: Reset to max health.");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var text in floatingTexts)
            {
                string dmgStr = text.DamageAmount.ToString();
                Vector2 pos = text.Position;
                Color color = Type == EntityType.Player ? Color.Red : Color.OrangeRed;
                spriteBatch.DrawString(myFont, dmgStr, pos, color);
            }
            base.Draw(spriteBatch);
        }
    }

    // For Damage number indicator
    internal class FloatingDamageText
    {
        public int DamageAmount;
        public Vector2 Position;
        public float Timer;

        public FloatingDamageText(int amount, Vector2 startPosition)
        {
            DamageAmount = amount;
            Position = startPosition;
            Timer = 1.0f; // Show for 1 second
        }
    }
}
