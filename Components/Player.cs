using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
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
        public bool hasMovedThisTurn = false;
        public bool playerMovedOntoEnemyTile { get; private set; }

        public Vector2 LastMovementDirection { get; private set; } = Vector2.UnitX; // Default facing right

        public Player() { }
        public Player(TileMap tileMap)
        {
            this.tileMap = tileMap;
        }

        // ---------- METHODS ---------- //

        // Initializes the player by finding the map system and tile map.
        public override void Start()
        {
            // null checks & component assignment
            globals = Globals.Instance; // globals
            
            playerSprite = GameObject.GetComponent<Sprite>(); // sprite
            if (playerSprite == null)
            {
                Debug.WriteLine("Player: Sprite component is NULL!");
            }

            healthSystem = GameObject.GetComponent<HealthSystem>() ?? GameObject.FindObjectOfType<HealthSystem>(); // HealthSystem
            globals._mapSystem = GameObject.FindObjectOfType<MapSystem>();  // mapsystem
            tileMap = globals._mapSystem?.Tilemap; // TileMap

            Debug.WriteLine("Player: Waiting for map initialization and start position...");
            MoveToStartTile();
        }

        // Updates the player's position based on input, checking for obstacles before moving.
        public override void Update(float deltaTime)
        {
            if (hasMovedThisTurn || !Combat.Instance.isPlayerTurn) return;

            if (tileMap == null)  
            {
                tileMap = globals._mapSystem?.Tilemap;
                if (tileMap == null) return;
            }

            // Input
            Vector2 currentPos = GameObject.Position;
            Vector2 targetPos = currentPos;
            bool moved = false;
            Vector2 movementDirection = Vector2.Zero;

            KeyboardState KeyboardState = Keyboard.GetState();

            if (KeyboardState.IsKeyDown(Keys.W))
            {
                //Debug.WriteLine($"Player: moving UP");
                targetPos.Y -= tileSize;
                moved = true;
                movementDirection = -Vector2.UnitY;
            }
            if (KeyboardState.IsKeyDown(Keys.A))
            {
                //Debug.WriteLine($"Player: moving LEFT");
                targetPos.X -= tileSize;
                moved = true;
                movementDirection = -Vector2.UnitX;
            }
            if (KeyboardState.IsKeyDown(Keys.S))
            {
                //Debug.WriteLine($"Player: moving DOWN");
                targetPos.Y += tileSize;
                moved = true;
                movementDirection = Vector2.UnitY;
            }
            if (KeyboardState.IsKeyDown(Keys.D))
            {
                //Debug.WriteLine($"Player: moving RIGHT");
                targetPos.X += tileSize; 
                moved = true;
                movementDirection = Vector2.UnitX;
            }

            if (moved && !hasMovedThisTurn)
            {
                // Update last movement direction
                if (movementDirection != Vector2.Zero)
                {
                    LastMovementDirection = movementDirection;
                }
                // Convert target position to tile coordinates
                Point targetTilePos = GetTileCoordinates(targetPos);

                // Check if target tile is an obstacle before moving
                if (!IsObstacle(targetTilePos))
                {
                    GameObject.Position = targetPos;
                    Debug.WriteLine($"Player: moved to position - {targetPos}");
                    hasMovedThisTurn = true;
                    // Combat
                    CheckForEnemy(targetTilePos);
                    // If the player successfully moves, advance the turn
                    Combat.Instance.AdvanceToNextTurn();
                }
                if (IsExit(targetTilePos))
                {
                    GameObject.Position = targetPos;
                    hasMovedThisTurn = true;
                    // gen rand next level
                    globals._mapSystem.LoadNextLevel();
                    // Advance turn after moving to exit
                    Combat.Instance.AdvanceToNextTurn();
                }
            }            
        }

        // Convert world position to tile coordinates
        private Point GetTileCoordinates(Vector2 worldPosition)
        {
            return new Point(
                (int)(worldPosition.X / (tileSize * spriteScale)),
                (int)(worldPosition.Y / (tileSize * spriteScale)));
        }
        
        public Vector2 GetPlayerFacingDirection() // get player's facing direction
        {
            return LastMovementDirection;
        }

        // Check if the target tile contains an "obstacle" or "wall" tile
        private bool IsObstacle(Point tileCoordinates)
        {
            if (tileMap == null) return false;

            Sprite targetTile = tileMap.GetTileAt(tileCoordinates.X, tileCoordinates.Y);
            // check for floor & wall obstacles
            if (targetTile != null && targetTile.Texture == tileMap.obstacleTexture || targetTile.Texture == tileMap.wallTexture)
            {
                Debug.WriteLine($"Player: Obstacle at {tileCoordinates.X}, {tileCoordinates.Y}!");
                return true;
            }

            // Check for enemy tiles
            List<Enemy> enemies = GameObject.FindObjectOfType<Enemy>().GetEnemies();
            foreach (Enemy enemy in enemies)
            {
                if (enemy.GameObject != null)
                {
                    Vector2 enemyTile = enemy.GameObject.Position / 32;
                    Point enemyTilePoint = new Point((int)enemyTile.X, (int)enemyTile.Y);

                    if (enemyTilePoint == tileCoordinates)
                    {
                        Debug.WriteLine($"Player: Enemy tile at {tileCoordinates.X}, {tileCoordinates.Y}!");
                        return true;
                    }
                }
            }

            return false;
        }

        // Checks if the player's current position is on an "exit" tile.
        public bool IsExit(Point playerPosition)
        {
            if (tileMap == null) return false;

            Sprite targetTile = tileMap.GetTileAt(playerPosition.X, playerPosition.Y);
            if (targetTile != null && targetTile.Texture == tileMap.exitTexture)
            {
                Debug.WriteLine($"Player: Exit found at {playerPosition.X}, {playerPosition.Y}!");
                return true;
            }
            return false;
        }

        // Finds the "start" tile and moves the player to it
        public void MoveToStartTile()
        {
            if (tileMap == null) return;

            for (int y = 0; y < tileMap.mapHeight; y++)
            {
                for (int x = 0; x < tileMap.mapWidth; x++)
                {
                    Sprite tile = tileMap.GetTileAt(x, y);
                    if (tile != null && tile.Texture == tileMap.startTexture)
                    {
                        GameObject.Position = new Vector2(x * tileSize, y * tileSize);
                        Debug.WriteLine($"Player: Spawned at start tile ({x}, {y}).");
                        return;
                    }
                }
            }
            Debug.WriteLine("Player: Start tile not found!");
        }

        // Turn-based system
        private bool CheckForEnemy(Point playerPosition) 
        {
            Debug.WriteLine($"Player: checking for enemy at {playerPosition.X}, {playerPosition.Y}");
            if (tileMap == null) return false;

            Sprite targetTile = tileMap.GetTileAt(playerPosition.X, playerPosition.Y); // change to surrounding 8 tiles

            if (targetTile != null && targetTile.Texture == tileMap.enemyTexture) // changed from enemy to other for testing debug| enemyTexture
            {
                Debug.WriteLine($"Player: enemy found at {playerPosition.X}, {playerPosition.Y}!"); // worked with floortexture
                playerMovedOntoEnemyTile = true;
                return true;
            }
            Debug.WriteLine("Player: no enemy found");
            return false;
        }
        public void ResetTurn()
        {
            // Reset movement state
            hasMovedThisTurn = false;
            playerMovedOntoEnemyTile = false;

            Debug.WriteLine("Player: Turn reset - ready to move again");
        }
        public void TakeDamage(int damage) => healthSystem.TakeDamage(damage);

        // Combat
        public void Attack(Enemy enemy)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(10);
            }
        }       
    }
}