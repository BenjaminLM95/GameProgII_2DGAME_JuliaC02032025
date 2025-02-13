using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class Player : Component
    {
        /// <summary>
        /// Responsible for handling player input, and updating the player's Position in Update()
        /// </summary>
  
        GameManager _gameManager;
        private TileMap tileMap;

        // ---------- VARIABLES ---------- //
        // Get/Use health properties from HealthSystem      
        private float speed = 300f;
        private int tileSize = 32;
        private int spriteScale = 1;

        public Player() { }
        public Player(TileMap tileMap)
        {
            this.tileMap = tileMap;
        }

        // ---------- METHODS ---------- //
        public override void Start() 
        {
            _gameManager = GameManager.Instance;

            if (_gameManager == null)
            {
                Debug.WriteLine("Player: _gameManager is NULL!");
                return;
            }

            _gameManager._mapSystem = GameObject.FindObjectOfType<MapSystem>();

            if (_gameManager._mapSystem == null)
            {
                Debug.WriteLine("Player: _gameManager._mapSystem is NULL! Trying to find it...");
                _gameManager._mapSystem = GameObject.FindObjectOfType<MapSystem>();  
            }

            tileMap = _gameManager._mapSystem?.Tilemap;  
        }

        /// <summary>
        /// Updates the player's state.
        /// </summary>
        public override void Update(float deltaTime)
        {
            if (tileMap == null)  // DEBUG: Retry if tileMap is still missing
            {
                tileMap = _gameManager._mapSystem?.Tilemap;
                if (tileMap != null)
                {
                    Debug.WriteLine("Player: tileMap assigned in Update!");
                }
                else
                {
                    Debug.WriteLine("Player: tileMap STILL NULL in Update!");
                    return; // Stop running logic until it's ready
                }
            }

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

        // Convert world currentPos to tile coordinates
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
    }
}