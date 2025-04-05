using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    /// <summary>
    /// Handles Map Behaviors, generation/loading, obstacles, and rules.
    /// </summary>
    internal class MapSystem : Component
    {
        private Globals globals;
        public TileMap Tilemap { get; private set; }

        // ---------- VARIABLES ---------- //

        private Random random = new Random();
        private int mapHeight = 15; // map HEIGHT
        private int mapWidth = 25; // map WIDTH

        private int tilePixelX = 32; // tile sprites are 16x16
        private int tilePixelY = 32;

        List<Vector2> emptyTiles = new List<Vector2>();

        public bool LevelChanged { get; private set; } = false;
        public int levelNumber = 1;

        // ---------- METHODS ---------- //

        public override void Start()
        {
            globals = Globals.Instance;
            // Create TileMap and initialize it
            Tilemap = new TileMap();
            Tilemap.LoadTextures(Globals.content);
            Tilemap.Initialize();

            // ------ MAP GENERATION ------ //
            //Debug.WriteLine("MapSystem: Generating random map.");
            GenerateMap();  // Random map generation
        }

        public void Update(GameTime gameTime)
        {
            // check if the player has reached the exit tile
            Player player = GameObject.FindObjectOfType<Player>();
        }

        // Loads the next level by clearing the current tilemap,
        // resetting the player position, and generating a new map.
        public void LoadNextLevel()
        {
            LevelChanged = true; // indicate that a new level is loading

            Tilemap.ClearTiles();

            // clearing lists of enemies
            Enemy._enemies.Clear();
            Enemy.AllEnemies.Clear();
            EnemySpawner.RespawnEnemies(5); // respawning TEST
            // implement boss levels, no other enemies spawn


            GenerateMap();
            levelNumber++; // next level
            Debug.WriteLine($"MapSystem: level number: {levelNumber}");

            Vector2 startTile = GetRandomEmptyTile(); // reset the player position to a random empty tile

            globals._player.MoveToStartTile();

            // Respawn items when a new level is generated
            Items itemsComponent = GameObject.FindObjectOfType<Items>();
            if (itemsComponent != null)
            {
                itemsComponent.SpawnItems(4);
            }
            else
            {
                Debug.WriteLine("MapSystem: Items component not found! Items will not respawn.");
            }           
        }

        // Generates a new random map, setting tile types for floors, obstacles, 
        // and designating random positions for start and exit tiles.
        public void GenerateMap(bool debug = false)
        {
            Tilemap.Initialize();

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Sprite tile = Tilemap.GetTileAt(x, y);
                    if (tile != null)
                    {
                        if (x == 0 || x == mapWidth - 1 || y == 0 || y == mapHeight - 1)
                        {
                            tile.Texture = Globals.content.Load<Texture2D>("wall"); // draw walls on edge
                        }
                        else
                        {
                            tile.Texture = Globals.content.Load<Texture2D>("floor"); // all else is floor
                        }
                    }
                }
            }
            // generating obstacle clusters
            for (int i = 0; i < 10; i++)
            {
                int obsX = random.Next(1, mapWidth - 4);
                int obsY = random.Next(1, mapHeight - 4);
                int obstacleWidth = random.Next(2, 5);
                int obstacleHeight = random.Next(2, 5);

                if (CanFitObstacle(obsX, obsY, obstacleWidth, obstacleHeight))
                {
                    PlaceObstacle(obsX, obsY, obstacleWidth, obstacleHeight);
                }
            }

            // Find random positions for start and exit tiles
            Vector2 startTile = GetRandomEmptyTile(false);
            Vector2 exitTile;

            // Keep finding a different exit tile until it's not the same as the start
            do
            {
                exitTile = GetRandomEmptyTile(false);
            } while (exitTile == startTile);

            // Set the start and exit tiles
            Sprite startSprite = Tilemap.GetTileAt((int)startTile.X, (int)startTile.Y);
            Sprite exitSprite = Tilemap.GetTileAt((int)exitTile.X, (int)exitTile.Y);

            if (startSprite != null)
                startSprite.Texture = Globals.content.Load<Texture2D>("start");

            if (exitSprite != null)
                exitSprite.Texture = Globals.content.Load<Texture2D>("exit");

            // Set player position to the start tile
            Player player = GameObject.FindObjectOfType<Player>();
            if (player != null)
            {
                if(debug) Debug.WriteLine($"MapSystem: Player position set to {player.GameObject.Position.X}, {player.GameObject.Position.Y}.");
                player.GameObject.Position = new Vector2(startTile.X, startTile.Y);
            }
            else if (player == null) {
                if (debug) Debug.WriteLine($"MapSystem: Player is NULL.");
            }

            if (debug) Debug.WriteLine("MapSystem: Random map generated successfully.");
        }
        public override void Draw(SpriteBatch spriteBatch) // take out override?
        {
            Tilemap.Draw(spriteBatch);
        }

        public Vector2 GetStartTilePosition()
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Sprite tile = Tilemap.GetTileAt(x, y);
                    if (tile != null && tile.Texture == Tilemap.startTexture)
                    {
                        return new Vector2(x * tilePixelX, y * tilePixelY);
                    }
                }
            }

            // If start tile not found, use the position from GetRandomEmptyTile()
            Vector2 startTile = GetRandomEmptyTile();
            return new Vector2(startTile.X * tilePixelX, startTile.Y * tilePixelY);
        }

        #region Obstacle Rules
        private bool CanPlaceObstacle(int x, int y)
        {
            // Check if the tile is not already occupied by another obstacle and is within the map bounds
            if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
                return false;

            Sprite tile = Tilemap.GetTileAt(x, y);
            return tile.Texture.Name == "floor"; // Only place obstacles where there are floor tiles
        }

        private bool CanFitObstacle(int x, int y, int width, int height)
        {
            // Check if the obstacle fits within the map boundaries
            if (x + width > mapWidth || y + height > mapHeight)
                return false;

            // Check if the area is empty (no obstacles or boundaries)
            for (int i = y; i < y + height; i++)
            {
                for (int j = x; j < x + width; j++)
                {
                    if (!CanPlaceObstacle(j, i)) // If any spot is not valid, return false
                        return false;
                }
            }
            return true;
        }

        private void PlaceObstacle(int x, int y, int width, int height)
        {
            for (int i = y; i < y + height; i++)
            {
                for (int j = x; j < x + width; j++)
                {
                    // Mark the tiles as obstacles
                    Sprite tile = Tilemap.GetTileAt(j, i);
                    tile.Texture = Globals.content.Load<Texture2D>("obstacle");
                }
            }
        }
        #endregion

        // Finds a random empty tile (not an obstacle) on the map.
        public Vector2 GetRandomEmptyTile(bool convertToPixelPosition = true, bool debug = false)
        {
            if (Tilemap == null)
            {
                if (debug) Debug.WriteLine("MapSystem: Tilemap is null!");
                return new Vector2(-1, -1);
            }

            int maxAttempts = mapWidth * mapHeight; // Prevent infinite loop
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                int x = random.Next(mapWidth);
                int y = random.Next(mapHeight);
                Sprite tile = Tilemap.GetTileAt(x, y);

                if (tile == null)
                {
                    if (debug) Debug.WriteLine($"MapSystem: Tile at ({x},{y}) is null!");
                    attempts++;
                    continue;
                }

                // More robust null-safe projSprite check
                if (tile.Texture != null && tile.Texture.Name == "floor")
                {
                    if (convertToPixelPosition)
                        return new Vector2(x * 32, y * 32); // Convert to pixel coordinates
                    else
                        return new Vector2(x, y);
                }

                attempts++;
            }

            if (debug) Debug.WriteLine("MapSystem: Could not find empty tile after multiple attempts!");
            return new Vector2(-1, -1);
        }

        public void ResetLevelFlag()
        {
            LevelChanged = false; // Reset flag after items have been re-spawned
        }
    }
}
