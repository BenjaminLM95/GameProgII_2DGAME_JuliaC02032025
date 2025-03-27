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
        public static Combat _instance { get; private set; }
        public static Combat Instance => _instance ??= new Combat();
        public Combat()
        {
            _instance = this;
        }

        private Globals globals;
        private Player player;

        public bool isPlayerTurn { get; private set; } = true;
        private const int TILE_SIZE = 32;

        private Texture2D turnIndicatorTexture;

        private List<GameObject> turnTakers = new List<GameObject>();
        private int currentTurnIndex = 0;

        // ---------- METHODS ---------- //

        public override void Start()
        {
            Debug.WriteLine("Combat: START");
            globals = Globals.Instance;

            if (Globals.content != null)
            {
                try {
                    turnIndicatorTexture = Globals.content.Load<Texture2D>("turnIndicator"); 
                    //Texture2D turnIndicatorTexture = globals._tileMap.turnIndicatorTexture;
                }
                catch (Exception ex) {
                    Debug.WriteLine($"Combat: Failed to load turn indicator texture - {ex.Message}");
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
        }
        // Adds valid GameObjects through their components to a list of turn takers
        private void AddTurnTaker(GameObject turnTakerObj)
        {
            if (turnTakerObj == null)
            {
                Debug.WriteLine("Combat: Attempted to add null GameObject to turn takers");
                return;
            }

            // Verify the GameObject has the required components
            var player = turnTakerObj.GetComponent<Player>();
            var enemy = turnTakerObj.GetComponent<Enemy>();

            if (player == null && enemy == null)
            {
                Debug.WriteLine("Combat: GameObject is not a Player or Enemy");
                return;
            }

            // Check if the GameObject is already in the list to avoid duplicates
            if (!turnTakers.Contains(turnTakerObj))
            {
                turnTakers.Add(turnTakerObj);
                Debug.WriteLine($"Combat: Added {(player != null ? "Player" : "Enemy")} to turn takers");
            }
            else {
                Debug.WriteLine("Combat: GameObject already in turn takers list");
            }
        }
        // Adds player & enemy specifically to turn takers
        private void InitializeTurnTakers()
        {
            turnTakers.Clear();

            // Add Player
            if (globals._player?.GameObject != null)
            {
                AddTurnTaker(globals._player.GameObject);
            }
            else {
                Debug.WriteLine("Combat: Player is NOT properly initialized!");
            }

            // Add Enemies
            if (globals._enemy != null)
            {
                var enemyList = globals._enemy.GetEnemies(); // get multiple enemies
                if (enemyList != null)
                {
                    foreach (var enemy in enemyList)
                    {
                        if (enemy?.GameObject != null)
                        {
                            AddTurnTaker(enemy.GameObject);
                        }
                    }
                }
            }

            if (turnTakers.Count == 0)
            {
                Debug.WriteLine("Combat: NO TURN TAKERS FOUND - CHECK INITIALIZATION!");
            }
            else {
                Debug.WriteLine($"Combat: Initialized {turnTakers.Count} turn takers.");
            }
        }
        // manages turn alternation by changing the index
        // and updating the current entity to decide which turn method to run
        private void TurnManager()
        {
            if (turnTakers.Count == 0)
            {
                Debug.WriteLine("Combat: No turn takers available. Reinitializing...");
                InitializeTurnTakers();
                return;
            }

            if (currentTurnIndex >= turnTakers.Count)
            {
                Debug.WriteLine($"Combat: Current turn index {currentTurnIndex} is out of bounds. Resetting.");
                currentTurnIndex = 0;
            }

            var currentTurnObject = turnTakers[currentTurnIndex];
            //Debug.WriteLine($"Combat: Current turn object is {currentTurnObject.GetType().Name}");

            var currentEntity = currentTurnObject?.GetComponent<Component>();

            if (currentEntity == null)
            {
                Debug.WriteLine("Combat: Invalid entity at turn index. Advancing to next turn.");
                AdvanceToNextTurn();
                return;
            }

            // Update the isPlayerTurn flag based on the current entity
            isPlayerTurn = currentEntity is Player;

            Debug.WriteLine($"Combat: {currentEntity.GetType().Name}'s turn.");

            if (currentEntity is Player player)
            {
                PlayerTurn(player);
            }
            else if (currentEntity is Enemy enemy) {
                EnemyTurn(enemy);
            }
        }
        // cycles through turntakers in index, resets turns
        public void AdvanceToNextTurn()
        {
            if (turnTakers == null || turnTakers.Count == 0)
            {
                Debug.WriteLine("Combat: Cannot advance turn - no turn takers");
                return;
            }

            currentTurnIndex = (currentTurnIndex + 1) % turnTakers.Count;

            // Reset movement for player when it's their turn
            if (turnTakers[currentTurnIndex].GetComponent<Player>() is Player playerComponent)
            {
                playerComponent.ResetTurn();
            }

            Debug.WriteLine($"Combat: Advanced to turn index {currentTurnIndex}"); // !! not showing up
        }

        private void PlayerTurn(Player player)
        {
            Debug.WriteLine("Combat: PlayerTurn() called");
            TurnIndicator(player.GameObject.Position); // turn indicator
        }

        private void EnemyTurn(Enemy enemy)
        {
            Debug.WriteLine($"Combat: EnemyTurn called for enemy at {enemy.GameObject.Position}");
            TurnIndicator(enemy.GameObject.Position); // turn indicator

            Player player = globals._player;
            if (enemy.IsNextToPlayer(player))
            {
                enemy.Attack(player);
            }
            else
            {
                enemy.MoveTowardsPlayer(player);
            }

            AdvanceToNextTurn();
        }
        // draw turn indicator
        private void TurnIndicator(Vector2 position) // not drawing?
        {
            if (turnIndicatorTexture == null)
            {
                Debug.WriteLine("Combat: Turn indicator texture is NULL!");
                return;
            }

            try
            {
                if (Globals.spriteBatch == null)
                {
                    Debug.WriteLine("Combat: SpriteBatch is NULL when trying to draw turn indicator!");
                    return;
                }

                Debug.WriteLine($"Combat: Attempting to draw turn indicator at {position}");

                Globals.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                Globals.spriteBatch.Draw(
                    turnIndicatorTexture,
                    new Rectangle(
                        (int)position.X,
                        (int)position.Y - turnIndicatorTexture.Height,
                        turnIndicatorTexture.Width,
                        turnIndicatorTexture.Height
                    ),
                    Color.White
                );
                Globals.spriteBatch.End();

                Debug.WriteLine("Combat: Turn indicator drawing completed successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Combat: Error drawing turn indicator - {ex.Message}");
            }
        }
    }
}
// NOTE: turnIndicator not working, says drawn sucessfully but not seeing it, move forward in draw?