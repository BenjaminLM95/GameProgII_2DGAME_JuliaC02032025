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

        private Globals gameManager;
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
            gameManager = Globals.Instance;

            if (Globals.content != null)
            {
                try {
                    turnIndicatorTexture = Globals.content.Load<Texture2D>("turnIndicator");
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

        private void AddTurnTaker(GameObject turnTakerObj)
        {
            if (turnTakerObj == null)
            {
                Debug.WriteLine("Combat: Attempted to add null GameObject to turn takers");
                return;
            }

            // Verify the GameObject has the required components
            var component = turnTakerObj.GetComponent<Component>();
            if (component == null)
            {
                Debug.WriteLine("Combat: GameObject does not have a valid Component for turn taking");
                return;
            }

            // Check if the GameObject is already in the list to avoid duplicates
            if (!turnTakers.Contains(turnTakerObj))
            {
                turnTakers.Add(turnTakerObj);
                Debug.WriteLine($"Combat: Added {component.GetType().Name} to turn takers");
            }
            else {
                Debug.WriteLine("Combat: GameObject already in turn takers list");
            }
        }

        private void InitializeTurnTakers()
        {
            turnTakers.Clear();

            // Add Player
            if (gameManager._player?.GameObject != null)
            {
                AddTurnTaker(gameManager._player.GameObject);
            }
            else {
                Debug.WriteLine("Combat: Player is NOT properly initialized!");
            }

            // Add Enemies
            if (gameManager._enemy != null)
            {
                var enemyList = gameManager._enemy.GetEnemies();
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
                Debug.WriteLine("Combat: NO TURN TAKERS FOUND - CHECK INITIALIZATION!");
            }
            else {
                Debug.WriteLine($"Combat: Initialized {turnTakers.Count} turn takers.");
            }
        }

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
            Debug.WriteLine($"Combat: Current turn object is {currentTurnObject.GetType().Name}");

            var currentEntity = currentTurnObject?.GetComponent<Component>();

            if (currentEntity == null)
            {
                Debug.WriteLine("Combat: Invalid entity at turn index. Advancing to next turn.");
                AdvanceToNextTurn();
                return;
            }

            // Update the isPlayerTurn flag based on the current entity
            isPlayerTurn = currentEntity is Player;

            Debug.WriteLine($"Combat: {currentEntity.GetType().Name}'s turn");

            if (currentEntity is Player player)
            {
                PlayerTurn(player);
            }
            else if (currentEntity is Enemy enemy) {
                EnemyTurn(enemy);
            }
        }

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

            Debug.WriteLine($"Combat: Advanced to turn index {currentTurnIndex}");
        }

        private void PlayerTurn(Player player)
        {
            Debug.WriteLine("Combat: Player's turn");
            TurnIndicator(player.GameObject.Position);
        }

        private void EnemyTurn(Enemy enemy)
        {
            Debug.WriteLine("Combat: Enemy's turn");
            TurnIndicator(enemy.GameObject.Position);

            Player player = gameManager._player;
            if (enemy.IsNextToPlayer(player))
            {
                enemy.Attack(player);
            }
            else {
                enemy.MoveTowardsPlayer(player);
            }
            AdvanceToNextTurn();
        }

        private void TurnIndicator(Vector2 position)
        {
            //Globals.spriteBatch.Draw(
            //    turnIndicatorTexture,
            //    new Vector2(position.X, position.Y - 32),
            //    Color.White
            //);
            Debug.WriteLine("Combat: Turn indicator drawn");
        }
    }
}
/*
 * private void TurnManager() 
        {
            // list of GameObjects turnTakers (includes player & all enemies)
            // index currentTurn = 0 or -1
            // Update (entry) every frame
            // - if valid index? (>=0 and < turnTakers.Count)
            // - if valid check if 
            //      has taken turn?
            //      has turn ended?
            // if both are true
            //      change to next index
            //      call take turn ()
            // nothing to do (after has taken turn & has turn ended both have happened)
            // else: find next valid
            // set has taken turn to false & has turn ended to false (after change to next index and find next valid)
        }
*/
