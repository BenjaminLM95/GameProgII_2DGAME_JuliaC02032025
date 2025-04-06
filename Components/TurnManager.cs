using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal interface ITurnTaker 
    {
        void StartTurn(TurnManager manager);
    }
    internal class TurnManager : Component
    {
        public static TurnManager Instance { get; private set; }

        private List<ITurnTaker> turnTakers = new List<ITurnTaker>();
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
        private Enemy enemy;
        private Globals globals;

        // Game state tracking
        private bool isGameOver = false;

        // Event for UI updates (can use turnindicator)
        public event Action<string, int> OnTurnChanged;
        public event Action OnGameOver;

        // Turn indicator
        private Texture2D turnIndicatorTexture;
        private const int TILE_SIZE = 32;

        public override void Start()
        {
            globals = Globals.Instance;
            if (globals == null)
            {
                Debug.WriteLine("TurnManager: Globals is NULL!");
                return;
            }
            LoadTurnIndicatorTexture();
            Initialize();
        }

        private void LoadTurnIndicatorTexture()
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

        private void Initialize()
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

            List<Enemy> enemies = GameObject.FindObjectOfType<Enemy>().GetEnemies();

            if (enemies.Count == 0)
            {
                Debug.WriteLine("TurnManager: No enemies found!");
            }
            else
            {
                Debug.WriteLine($"TurnManager: Found {enemies.Count} enemies");
                foreach (Enemy enemy in enemies)
                {
                    Debug.WriteLine($"TurnManager Enemy: {enemy.Type}");
                    if (enemy != null && enemy is ITurnTaker)
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

            // Check for player death - send off to healthsystem to handle death logic
            HealthSystem healthSystem = player.GameObject.GetComponent<HealthSystem>();
            if (player != null && healthSystem.CurrentHealth <= 0)
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
            List<Enemy> enemies = enemy.GetEnemies(); // crashing when payer dies
            bool newEnemiesFound = false;

            foreach (Enemy enemy in enemies)
            {
                if (enemy != null && enemy is ITurnTaker && !turnTakers.Contains(enemy))
                {
                    RegisterTurnTaker(enemy);
                    newEnemiesFound = true;
                }
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

        private void CleanupTurnTakers()
        {
            bool removedAny = false;
            // remove any null turn takers or dead enemies
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

