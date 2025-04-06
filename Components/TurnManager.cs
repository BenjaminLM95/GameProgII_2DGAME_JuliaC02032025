using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal interface ITurnTaker // any components that are turn takers 'inherit' this & need StartTurn()
    {
        void StartTurn(TurnManager manager);
    }
    internal class TurnManager : Component
    {
        // ---------- VARIABLES ---------- //
        public static TurnManager Instance { get; private set; }

        public List<ITurnTaker> turnTakers = new List<ITurnTaker>();
        private int currentTurnIndex = 0;
        private ITurnTaker currentTurnTaker;

        // Turn status
        public bool IsPlayerTurn { get; private set; }
        public int TurnCount { get; private set; } = 0;

        // Turn delay
        private float turnDelay = 0.5f;
        private float currentTurnTimer = 0f;
        private bool isWaitingForNextTurn = false;
        private ITurnTaker nextTurnTaker = null;

        // References
        private Player player;
        //private Enemy enemy;
        private Globals globals;
        private HealthSystem healthSystem;

        // Game state tracking
        public bool isGameOver = false;

        // Event for UI updates (can use turnindicator)
        public event Action<string, int> OnTurnChanged;
        public event Action OnGameOver;

        // Turn indicator
        private Texture2D turnIndicatorTexture;
        private const int TILE_SIZE = 32;

        // ---------- METHODS ---------- //
        public override void Start()
        {
            globals = Globals.Instance;
            if (globals == null)
            {
                Debug.WriteLine("TurnManager: Globals is NULL!");
                return;
            }
            healthSystem = Globals.Instance._healthSystem;

            LoadTurnIndicatorTexture();
            Initialize();
        }

        private void LoadTurnIndicatorTexture() // Load Textures
        {
            try
            {
                turnIndicatorTexture = Globals.content.Load<Texture2D>("turnIndicator");
                Debug.WriteLine("TurnManager: Turn indicator texture loaded successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"TurnManager: Failed to load turn indicator texture - {ex.Message}");
            }
        }

        private void Initialize() // first pass to get all turn tkers
        {
            Debug.WriteLine("TurnManager: Initializing turn order...");
            isGameOver = false;

            // Find player
            player = GameObject.FindObjectOfType<Player>();
            if (player == null)
            {
                Debug.WriteLine("TurnManager: Player not found!");
                return;
            }

            RegisterTurnTaker(player);

            Enemy enemyComponent = GameObject.FindObjectOfType<Enemy>();
            if (enemyComponent != null)
            {
                List<Enemy> enemies = enemyComponent.GetEnemies();

                // Add all enemies as turn takers
                foreach (Enemy enemy in enemies)
                {
                    if (enemy != null)
                    {
                        RegisterTurnTaker(enemy);
                    }
                }
            }

            Debug.WriteLine($"TurnManager: Starting first turn with {turnTakers.Count} turn takers");
            StartNextTurn();
        }

        public override void Update(float deltaTime)
        {
            // If game is over, don't process turns
            if (isGameOver)
            {
                return;
            }

            if (player == null)
            {
                Debug.WriteLine("TurnManager: Player is null during update");
                return;
            }
            var healthSystem = player.GameObject?.GetComponent<HealthSystem>();
            if (healthSystem == null)
            {
                Debug.WriteLine("TurnManager: Player has no HealthSystem!");
                return;
            }

            if (healthSystem.CurrentHealth <= 0)
            {
                isGameOver = true;
                healthSystem.HandlePlayerDeath();
                return;
            }

            CleanupTurnTakers();
            if (isWaitingForNextTurn)
            {
                currentTurnTimer += deltaTime;

                // if delay is complete, start the next turn
                if (currentTurnTimer >= turnDelay)
                {
                    isWaitingForNextTurn = false;
                    currentTurnTimer = 0f;

                    ActivateNextTurn();
                }
            }
            
            if (turnTakers.Count <= 1) // only player is left
            {
                // check if enemies need respawning
                RecheckEnemies();
            }
        }

        private void RecheckEnemies() // for newly spawned enemies 
        {
            bool newEnemiesFound = false;

            // Use the static lists directly instead of finding the first enemy
            if (Enemy.AllEnemies != null && Enemy.AllEnemies.Count > 0)
            {
                foreach (GameObject enemyObj in Enemy.AllEnemies)
                {
                    if (enemyObj != null)
                    {
                        Enemy enemy = enemyObj.GetComponent<Enemy>();
                        if (enemy != null && enemy is ITurnTaker && !turnTakers.Contains(enemy))
                        {
                            RegisterTurnTaker(enemy);
                            newEnemiesFound = true;
                        }
                    }
                }
            }
            else if (Enemy._enemies != null && Enemy._enemies.Count > 0)
            {
                // Backup check using the _enemies list
                foreach (Enemy enemy in Enemy._enemies)
                {
                    if (enemy != null && enemy is ITurnTaker && !turnTakers.Contains(enemy))
                    {
                        RegisterTurnTaker(enemy);
                        newEnemiesFound = true;
                    }
                }
            }
            else
            {
                Debug.WriteLine("TurnManager: No enemies found to register (this is expected if none have spawned yet)");
            }

            if (newEnemiesFound)
            {
                Debug.WriteLine("TurnManager: Registered new enemies");
            }
        }

        public void RegisterTurnTaker(ITurnTaker turnTaker)
        {
            if (!turnTakers.Contains(turnTaker))
            {
                turnTakers.Add(turnTaker);
                Debug.WriteLine($"TurnManager: Registered {turnTaker.GetType().Name} as turn taker");
            }
        }

        public void RemoveTurnTaker(ITurnTaker taker)
        {
            if (turnTakers.Contains(taker))
            {
                turnTakers.Remove(taker);
            }

            // if the one we removed was about to take a turn, skip it
            if (currentTurnIndex >= turnTakers.Count)
            {
                currentTurnIndex = 0;
            }
        }

        private void CleanupTurnTakers() // remove any null turn takers or dead enemies
        {
            bool removedAny = false;

            for (int i = turnTakers.Count - 1; i >= 0; i--)
            {
                bool shouldRemove = false;

                // check for null
                if (turnTakers[i] == null)
                {
                    shouldRemove = true;
                    Debug.WriteLine($"TurnManager: Removing null turn taker at index {i}");
                }
                // check for dead enemies
                else if (turnTakers[i] is Enemy enemy && enemy.IsDead())
                {
                    shouldRemove = true;
                    Debug.WriteLine($"TurnManager: Removing dead enemy turn taker ({enemy.Type}) at index {i}");
                }

                if (shouldRemove)
                {
                    turnTakers.RemoveAt(i);
                    removedAny = true;

                    // adjust current index if needed
                    if (i <= currentTurnIndex && currentTurnIndex > 0)
                    {
                        currentTurnIndex--;
                    }
                }
            }
            if (removedAny && turnTakers.Count > 0 && currentTurnIndex < turnTakers.Count)
            {
                currentTurnTaker = turnTakers[currentTurnIndex];
            }
        }

        // ______________________________________________________ Turn Handling Below, Turn Takers above

        // Called by turn takers when they complete their turn
        public void EndTurn()
        {
            // handle player-specific turn reset
            if (currentTurnTaker is Player playerTurnTaker)
            {
                playerTurnTaker.ResetTurn();
            }

            // move to the next turn taker
            PrepareNextTurn();
        }

        private void PrepareNextTurn()
        {
            CleanupTurnTakers(); // clean up any null or dead enemies

            if (turnTakers.Count == 0)
            {
                Debug.WriteLine("TurnManager: No turn takers registered!");
                return;
            }

            // calculate the next turn index but don't activate yet
            int nextIndex = (currentTurnIndex + 1) % turnTakers.Count;
            nextTurnTaker = turnTakers[nextIndex];

            // start delay timer
            isWaitingForNextTurn = true;
            currentTurnTimer = 0f;

            Debug.WriteLine($"TurnManager: Waiting {turnDelay} seconds before next turn");
        }

        private void ActivateNextTurn()
        {
            // update the turn index to the next turn taker
            currentTurnIndex = (currentTurnIndex + 1) % turnTakers.Count;
            currentTurnTaker = turnTakers[currentTurnIndex];

            IsPlayerTurn = currentTurnTaker is Player;
            if (currentTurnTaker is Player player)
            {
                player.ResetTurn();
                player.StartTurn(this);
            }

            if (IsPlayerTurn && currentTurnIndex == 0)
            {
                TurnCount++;
            }

            string entityType = IsPlayerTurn ? "Player" : (currentTurnTaker is Enemy enemy ? $"{enemy.Type}" : "Unknown");
            Debug.WriteLine($"TurnManager: Turn {TurnCount} - {entityType}'s turn");

            // notify listeners
            OnTurnChanged?.Invoke(entityType, TurnCount);

            currentTurnTaker.StartTurn(this);
        }

        private void StartNextTurn()
        {
            PrepareNextTurn();
        }

        // Reset the turn system
        public void ResetTurns()
        {
            Debug.WriteLine("TurnManager: Resetting turn system");

            turnTakers.Clear();
            currentTurnIndex = 0;
            TurnCount = 0;
            isWaitingForNextTurn = false;
            currentTurnTimer = 0f;

            Initialize();
        }
        public float TurnDelay
        {
            get { return turnDelay; }
            set { turnDelay = Math.Max(0, value); } 
        }

        // Helper method to get the entity type name for UI display
        public string GetCurrentTurnEntityName()
        {
            if (currentTurnTaker is Player)
            {
                return "Player";
            }
            else if (currentTurnTaker is Enemy enemy)
            {
                return enemy.Type.ToString();
            }
            return "Unknown";
        }

        // ---------- Turn indicator ---------- //
        public void DrawTurnIndicator(bool debug = false)
        {
            if (turnIndicatorTexture == null)
            {
                if (debug) Debug.WriteLine("TurnManager: Turn indicator texture is NULL!");
                return;
            }

            try
            {
                if (turnTakers.Count > 0 && currentTurnIndex < turnTakers.Count && currentTurnTaker != null)
                {
                    bool isValid = true;
                    if (currentTurnTaker is Enemy enemy)
                    {
                        isValid = !enemy.IsDead();
                        if (debug && !isValid) Debug.WriteLine("TurnManager: Current turn taker is a dead enemy");
                    }

                    // only proceed if the turn taker is valid
                    if (isValid)
                    {
                        // get position from the current turn taker
                        Vector2 position = GetTurnTakerPosition(currentTurnTaker);

                        // debug logging
                        if (debug)
                        {
                            Debug.WriteLine($"Turn Indicator - Object: {currentTurnTaker.GetType().Name}");
                            Debug.WriteLine($"Turn Indicator - Position: {position}");
                            Debug.WriteLine($"Turn Indicator - Texture Size: {turnIndicatorTexture.Width}x{turnIndicatorTexture.Height}");
                        }

                        // draw with offset positioning
                        Globals.spriteBatch.Draw(
                            turnIndicatorTexture,
                            new Vector2(
                                position.X + (TILE_SIZE - turnIndicatorTexture.Width) / 2,
                                position.Y - turnIndicatorTexture.Height - 5
                            ),
                            Color.White
                        );
                    }
                    else if (debug)
                    {
                        Debug.WriteLine("TurnManager: Skipping turn indicator for invalid turn taker");
                    }
                }
            }
            catch (Exception ex)
            {
                if (debug) Debug.WriteLine($"TurnManager: Detailed turn indicator error - {ex.Message}");
            }
        }
        private Vector2 GetTurnTakerPosition(ITurnTaker turnTaker)
        {
            // handle different types of turn takers
            if (turnTaker is Player playerTurnTaker)
            {
                return playerTurnTaker.GameObject.Position;
            }
            else if (turnTaker is Enemy enemyTurnTaker)
            {
                return enemyTurnTaker.GameObject.Position;
            }

            return Vector2.Zero; // default position
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            CleanupTurnTakers();
            DrawTurnIndicator();
        }
    }
}

