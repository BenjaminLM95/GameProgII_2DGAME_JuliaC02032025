using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using GameProgII_2DGame_Julia_C02032025.Components;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal class Combat : Component
    {
        // ---------- INSTANCE ---------- //
        public static Combat _instance { get; private set; }
        public static Combat Instance => _instance ??= new Combat();
        public Combat()
        {
            _instance = this;
        }

        // ---------- REFERENCES ---------- //
        private Globals globals;
        private Player player;

        // ---------- VARIABLES ---------- //
        public bool isPlayerTurn { get; private set; } = true;
        private const int TILE_SIZE = 32;

        private Texture2D turnIndicatorTexture;

        private List<GameObject> turnTakers = new List<GameObject>();
        private int currentTurnIndex = 0;

        // damage indicator
        // Store the entity and how long to display the damage
        private Dictionary<GameObject, float> damageIndicators = new Dictionary<GameObject, float>(); 
        private const float dmgDisplayTime = 1.0f; // time in seconds to show damage
        private SpriteFont damageFont; // font to display damage text

        // ---------- METHODS ---------- //
        public override void Start()
        {
            Debug.WriteLine("Combat: Start()");
            globals = Globals.Instance;

            if (Globals.content != null)
            {
                try {
                    turnIndicatorTexture = Globals.content.Load<Texture2D>("turnIndicator");
                    damageFont = Globals.content.Load<SpriteFont>("DamageFont");
                }
                catch (Exception ex) {
                    Debug.WriteLine($"Combat: Failed to load turn indicator projSprite - {ex.Message}");
                }
            }
            InitializeTurnTakers();
        }

        public override void Update(float deltaTime)
        {
            if (turnTakers.Count > 0) // only run turn manager if there are turn takers
            {
                TurnManager();
            }

            // Update damage indicators
            List<GameObject> toRemove = new List<GameObject>();
            foreach (var kvp in damageIndicators)
            {
                damageIndicators[kvp.Key] -= deltaTime;

                if (damageIndicators[kvp.Key] <= 0) {
                    toRemove.Add(kvp.Key); // Mark for removal once time is up
                }
            }

            // Remove expired damage indicators
            foreach (var target in toRemove) {
                damageIndicators.Remove(target);
            }
        }

        // Adds valid GameObjects through their components to a list of turn takers
        private void AddTurnTaker(GameObject turnTakerObj, bool debug = false)
        {
            if (turnTakerObj == null) {
                if (debug) Debug.WriteLine("Combat: Attempted to add null GameObject to turn takers");
                return;
            }

            // Verify the GameObject has the required components
            var player = turnTakerObj.GetComponent<Player>();
            var enemy = turnTakerObj.GetComponent<Enemy>();

            if (player == null && enemy == null) {
                if (debug) Debug.WriteLine("Combat: GameObject is not a Player or Enemy");
                return;
            }

            // Check if the GameObject is already in the list to avoid duplicates
            if (!turnTakers.Contains(turnTakerObj))
            {
                turnTakers.Add(turnTakerObj);
                if (debug) Debug.WriteLine($"Combat: Added {(player != null ? "Player" : "Enemy")} to turn takers");
            }
            else {
                if (debug) Debug.WriteLine("Combat: GameObject already in turn takers list");
            }
        }
        // Adds player & enemy specifically to turn takers
        private void InitializeTurnTakers(bool debug = false)
        {
            turnTakers.Clear();

            // Add Player
            if (globals._player?.GameObject != null)
            {
                AddTurnTaker(globals._player.GameObject);
            }
            else {
                if (debug) Debug.WriteLine("Combat: Player is NOT properly initialized!");
            }

            // Add Enemies
            if (globals._enemy != null)
            {
                var enemyList = globals._enemy.GetEnemies(); // get multiple enemies
                if (enemyList != null)
                {
                    foreach (var enemy in enemyList)
                    {
                        if (enemy?.GameObject != null) {
                            AddTurnTaker(enemy.GameObject);
                        }
                    }
                }
            }

            if (turnTakers.Count == 0)
            {
                if (debug) Debug.WriteLine("Combat: NO TURN TAKERS FOUND - CHECK INITIALIZATION!");
            }
            else {
                if (debug) Debug.WriteLine($"Combat: Initialized {turnTakers.Count} turn takers.");
            }
        }
        // manages turn alternation by changing the index
        // and updating the current entity to decide which turn method to run
        private void TurnManager(bool debug = false)
        {
            if (turnTakers.Count == 0)
            {
                if (debug) Debug.WriteLine("Combat: No turn takers available. Reinitializing...");
                InitializeTurnTakers();
                return;
            }

            if (currentTurnIndex >= turnTakers.Count)
            {
                if (debug) Debug.WriteLine($"Combat: Current turn index {currentTurnIndex} is out of bounds. Resetting.");
                currentTurnIndex = 0;
            }

            var currentTurnObject = turnTakers[currentTurnIndex];
            //Debug.WriteLine($"Combat: Current turn object is {currentTurnObject.GetType().Name}");

            var currentEntity = currentTurnObject?.GetComponent<Component>();

            if (currentEntity == null)
            {
                if (debug) Debug.WriteLine("Combat: Invalid entity at turn index. Advancing to next turn.");
                AdvanceToNextTurn();
                return;
            }

            // Update the isPlayerTurn flag based on the current entity
            isPlayerTurn = currentEntity is Player;

            if (debug) Debug.WriteLine($"Combat: {currentEntity.GetType().Name}'s turn.");

            if (currentEntity is Player player)
            {
                PlayerTurn(player);
            }
            else if (currentEntity is Enemy enemy) {
                EnemyTurn(enemy);
            }
        }
        // cycles through turntakers in index, resets turns
        public void AdvanceToNextTurn(bool debug = false)
        {
            if (turnTakers == null || turnTakers.Count == 0)
            {
                if (debug) Debug.WriteLine("Combat: Cannot advance turn - no turn takers");
                return;
            }

            currentTurnIndex = (currentTurnIndex + 1) % turnTakers.Count;

            // Reset movement for player when it's their turn
            if (turnTakers[currentTurnIndex].GetComponent<Player>() is Player playerComponent)
            {
                playerComponent.ResetTurn();
            }

            if (debug) Debug.WriteLine($"Combat: Advanced to turn index {currentTurnIndex}"); // !! not showing up
        }

        private void PlayerTurn(Player player, bool debug = false) {
            if (debug) Debug.WriteLine("Combat: PlayerTurn() called");
        }

        private void EnemyTurn(Enemy enemy, bool debug = false)
        {
            if (debug) Debug.WriteLine($"Combat: EnemyTurn called for enemy at {enemy.GameObject.Position}");

            Player player = globals._player;
            if (enemy.IsNextToPlayer(player))
            {
                enemy.Attack(player); // attack if next to player
            }
            else {
                enemy.MoveTowardsPlayer(player); // move if not next to player
            }

            AdvanceToNextTurn();
        }
        // draw turn indicator
        public void DrawTurnIndicator(bool debug = false)
        {
            if (turnIndicatorTexture == null) {
                if (debug) Debug.WriteLine("Combat: Turn indicator projSprite is NULL!");
                return;
            }

            try
            {
                if (turnTakers.Count > 0 && currentTurnIndex < turnTakers.Count)
                {
                    GameObject currentTurnObject = turnTakers[currentTurnIndex];
                    Vector2 position = currentTurnObject.Position;

                    // Debug logging
                    if (debug)
                    {
                        Debug.WriteLine($"Turn Indicator - Object: {currentTurnObject.GetType().Name}");
                        Debug.WriteLine($"Turn Indicator - Position: {position}");
                        Debug.WriteLine($"Turn Indicator - Texture Size: {turnIndicatorTexture.Width}x{turnIndicatorTexture.Height}");
                    }

                    // Draw with offset positioning
                    Globals.spriteBatch.Draw(
                        turnIndicatorTexture,
                        new Vector2(
                            position.X + (TILE_SIZE - turnIndicatorTexture.Width) / 2,
                            position.Y - turnIndicatorTexture.Height
                        ),
                        Color.White
                    );
                }
            }
            catch (Exception ex) {
                if (debug) Debug.WriteLine($"Combat: Detailed turn indicator error - {ex.Message}");
            }
        }

        public void DisplayDamage(GameObject target, float damage)
        {
            if (!damageIndicators.ContainsKey(target))
            {
                damageIndicators.Add(target, dmgDisplayTime); // Set the display time for the damage text
            }

            // damageValues[target] = damage; // for displaying unique value, still not working
        }
        public void DrawDamageIndicator(int damage)
        {
            foreach (var kvp in damageIndicators)
            {
                GameObject target = kvp.Key;
                Vector2 position = target.Position;

                Vector2 textPosition = new Vector2(position.X, position.Y - TILE_SIZE); // Slightly above the entity

                //DisplayDamage(target, damage); // damage value passed from DisplayDamage()

                Globals.spriteBatch.DrawString(damageFont, damage.ToString(), textPosition, Color.Red);
            }
        }
    }
}
