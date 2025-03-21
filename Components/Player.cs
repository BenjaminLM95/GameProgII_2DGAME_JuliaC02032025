using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    /// <summary>
    /// Handles player input, movement, and interactions with the map (e.g., obstacles, exit).
    /// </summary>
    internal class Player : Component
    {
        Globals globals;
        private TileMap tileMap;
        private HealthSystem healthSystem;
        private Sprite playerSprite;

        // ---------- VARIABLES ---------- //

        private float speed = 300f;
        private int tileSize = 32;
        private int spriteScale = 1;

        // Turn based combat
        private bool hasMovedThisTurn = false;
        public bool playerMovedOntoEnemyTile { get; private set; }

        public Player() { }
        public Player(TileMap tileMap)
        {
            this.tileMap = tileMap;
        }

        // make constructor for health, position, base variables

        // ---------- METHODS ---------- //

        // Initializes the player by finding the map system and tile map.
        public override void Start()
        {
            // null checks & component assignment
            globals = Globals.Instance; // globals
            if (globals == null)
            {
                Debug.WriteLine("Player: globals is NULL!"); 
                return;
            }

            playerSprite = GameObject.GetComponent<Sprite>(); // sprite
            if (playerSprite == null)
            {
                Debug.WriteLine("Player: Sprite component is NULL!");
            }

            healthSystem = GameObject.GetComponent<HealthSystem>(); // health
            if (healthSystem == null)
            {
                healthSystem = GameObject.FindObjectOfType<HealthSystem>();
            }

            globals._mapSystem = GameObject.FindObjectOfType<MapSystem>();  // mapsystem
            if (globals._mapSystem == null)
            {
                Debug.WriteLine("Player: globals._mapSystem is NULL! Trying to find it..."); 
                globals._mapSystem = GameObject.FindObjectOfType<MapSystem>();
            }

            tileMap = globals._mapSystem?.Tilemap;
            Debug.WriteLine("Player: Waiting for map initialization and start position...");
        }

        // Updates the player's position based on input, checking for obstacles before moving.
        public override void Update(float deltaTime)
        {
            if (hasMovedThisTurn) return;

            if (tileMap == null)  // DEBUG: Retry if tileMap is still missing
            {
                tileMap = globals._mapSystem?.Tilemap;
                if (tileMap != null)
                {
                    Debug.WriteLine("Player: tileMap assigned in Update!");
                }
                else
                {
                    Debug.WriteLine("Player: tileMap STILL NULL in Update!");
                    return;
                }
            }

            // Input
            Vector2 currentPos = GameObject.Position;
            Vector2 targetPos = currentPos;
            bool moved = false;

            KeyboardState KeyboardState = Keyboard.GetState();

            if (KeyboardState.IsKeyDown(Keys.W))
            {
                Debug.WriteLine($"Player: moving UP");
                targetPos.Y -= tileSize;
                moved = true;
            }
            if (KeyboardState.IsKeyDown(Keys.A))
            {
                Debug.WriteLine($"Player: moving LEFT");
                targetPos.X -= tileSize;
                moved = true;
            }
            if (KeyboardState.IsKeyDown(Keys.S))
            {
                Debug.WriteLine($"Player: moving DOWN");
                targetPos.Y += tileSize;
                moved = true;
            }
            if (KeyboardState.IsKeyDown(Keys.D))
            {
                Debug.WriteLine($"Player: moving RIGHT");
                targetPos.X += tileSize; 
                moved = true;
            }

            if (moved && !hasMovedThisTurn)
            {
                // Convert target position to tile coordinates
                Point targetTilePos = GetTileCoordinates(targetPos);

                // Check if target tile is an obstacle before moving
                if (!IsObstacle(targetTilePos))
                {
                    GameObject.Position = targetPos;
                    Debug.WriteLine($"Player: moved to position - {targetPos}");
                    hasMovedThisTurn = true;
                    // Combat
                    CheckForEnemy();
                }
            }
        }
        public void Draw() // take out override?
        {
            
        }

        // Convert world position to tile coordinates
        private Point GetTileCoordinates(Vector2 worldPosition)
        {
            return new Point(
                (int)(worldPosition.X / (tileSize * spriteScale)),
                (int)(worldPosition.Y / (tileSize * spriteScale)));
        }

        // Check if the target tile contains an obstacle
        private bool IsObstacle(Point tileCoordinates)
        {
            if (tileMap == null) return false;

            Sprite targetTile = tileMap.GetTileAt(tileCoordinates.X, tileCoordinates.Y);

            if (targetTile != null && targetTile.Texture == tileMap.obstacleTexture)
            {
                Console.WriteLine($"Player: Obstacle at {tileCoordinates.X}, {tileCoordinates.Y}!");
                return true;
            }
            return false;
        }

        // Checks if the player's current position is on an exit tile.
        public bool IsExit(Vector2 playerPosition)
        {
            Point tileCoordinates = new Point((int)playerPosition.X / 32, (int)playerPosition.Y / 32);
            if (tileMap == null) return false;

            Sprite targetTile = tileMap.GetTileAt(tileCoordinates.X, tileCoordinates.Y);
            if (targetTile != null && targetTile.Texture == tileMap.exitTexture)
            {
                Console.WriteLine($"Exit at {tileCoordinates.X}, {tileCoordinates.Y}!");
                return true;
            }
            return false;
        }

        // Combat
        public void Attack(Enemy enemy)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(10);
            }
        }

        public void TakeDamage(int damage)
        {
            healthSystem.TakeDamage(damage);
        }

        // Turn-based system
        private void CheckForEnemy() // return Vector2 ?
        {
            // to be used in Combat.cs, if player is on enemy player takes turn
            // ref: Enemy.cs for enemies position
        }
        public void ResetTurn()
        {
            hasMovedThisTurn = false;
        }
    }
}