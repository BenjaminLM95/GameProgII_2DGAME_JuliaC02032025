using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    /// <summary>
    /// Handles player input, movement, and interactions with the map (e.g., obstacles, exit).
    /// </summary>
    internal class Player : Component
    {
        Globals globals;
        private TileMap tileMap;

        // ---------- VARIABLES ---------- //
         
        private float speed = 300f;
        private int tileSize = 32;
        private int spriteScale = 1;

        public Player() { }
        public Player(TileMap tileMap)
        {
            this.tileMap = tileMap;
        }

        // ---------- METHODS ---------- //

        // Initializes the player by finding the map system and tile map.
        public override void Start() 
        {
            globals = Globals.Instance;

            if (globals == null)
            {
                Debug.WriteLine("Player: globals is NULL!");
                return;
            }

            globals._mapSystem = GameObject.FindObjectOfType<MapSystem>();

            if (globals._mapSystem == null)
            {
                Debug.WriteLine("Player: globals._mapSystem is NULL! Trying to find it...");
                globals._mapSystem = GameObject.FindObjectOfType<MapSystem>();  
            }

            tileMap = globals._mapSystem?.Tilemap;  
        }

        // Updates the player's position based on input, checking for obstacles before moving.
        public override void Update(float deltaTime)
        {
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

            KeyboardState KeyboardState = Keyboard.GetState();

            if (KeyboardState.IsKeyDown(Keys.W)) targetPos.Y -= speed * deltaTime; // UP
            if (KeyboardState.IsKeyDown(Keys.A)) targetPos.X -= speed * deltaTime; // LEFT
            if (KeyboardState.IsKeyDown(Keys.S)) targetPos.Y += speed * deltaTime; // DOWN
            if (KeyboardState.IsKeyDown(Keys.D)) targetPos.X += speed * deltaTime; // RIGHT     

            // Convert target position to tile coordinates
            Point targetTilePos = GetTileCoordinates(targetPos);

            // Check if target tile is an obstacle before moving
            if (!IsObstacle(targetTilePos))
            {
                GameObject.Position = targetPos;
            }
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
                Console.WriteLine($"Obstacle at {tileCoordinates.X}, {tileCoordinates.Y}!");
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

        private void CheckForEnemy() // return Vector2 ?
        {
            // to be used in Combat.cs, if player is on enemy player takes turn
            // ref: Enemy.cs for enemies position
        }
    }
}