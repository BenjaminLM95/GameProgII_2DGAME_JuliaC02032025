using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using static System.Net.WebRequestMethods;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    /// <summary>
    /// Handles Map Behaviors, generation/loading, obstacles, and rules.
    /// </summary>
    internal class MapSystem : Component
    {
        private Globals _gameManager;
        public TileMap Tilemap { get; private set; }

        // ---------- VARIABLES ---------- //

        private Random random = new Random();
        private int mapHeight = 15; // map HEIGHT
        private int mapWidth = 25; // map WIDTH

        private int tilePixelX = 32; // tile sprites are 16x16
        private int tilePixelY = 32;

        List<Vector2> emptyTiles = new List<Vector2>();

        // ---------- METHODS ---------- //

        public override void Start()
        {
            // Create TileMap and initialize it
            Tilemap = new TileMap();
            Tilemap.LoadTextures(Globals.content);
            Tilemap.Initialize();

            // ---!!!--- MAP GENERATION ---!!!--- // <switch here
            Debug.WriteLine("MapSystem: Generating random map.");
            GenerateMap();  // Random map generation

            // Load map from.txt file
            //LoadMapFromFile("C:\\MY FILES\\Programming\\Unity Projects NSCC\\" + 
            //"GameProgII_2DGame_Julia_C02032025\\MyMaps\\Map1.txt");
            //LoadMapFromFile("C:\\Users\\W0517383\\Documents\\GitHub\\" +
            //  "GameProgII_2DGame_Julia_C02032025\\MyMaps\\Map1.txt");
        }

        public void Update(GameTime gameTime)
        {
            // Check if the player has reached the exit tile
            Player player = GameObject.FindObjectOfType<Player>();
            if (player != null && player.IsExit(player.GameObject.Position))  // Access Position through GameObject
            {
                // Clear current map and load the next level
                Tilemap.ClearTiles();
                GenerateMap();
                Debug.WriteLine("Level Transition: Player reached the exit!");
            }
        }

        // Loads the next level by clearing the current tilemap,
        // resetting the player position, and generating a new map.
        public void LoadNextLevel()
        {
            Tilemap.ClearTiles();

            // Reset the player position to a random empty tile
            Vector2 startTile = GetRandomEmptyTile();

            Player player = GameObject.FindObjectOfType<Player>();
            player.GameObject.Position =
                new Vector2(startTile.X * tilePixelX, startTile.Y * tilePixelY);

            GenerateMap();
        }

        // Generates a new random map, setting tile types for floors, obstacles, 
        // and designating random positions for start and exit tiles.
        public void GenerateMap()
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
            Vector2 startTile = GetRandomEmptyTile();
            Vector2 exitTile;

            // Keep finding a different exit tile until it's not the same as the start
            do
            {
                exitTile = GetRandomEmptyTile();
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
                Debug.WriteLine($"MapSystem: Player position set to {player.GameObject.Position.X}, {player.GameObject.Position.Y}.");
                player.GameObject.Position = new Vector2(startTile.X * tilePixelX, startTile.Y * tilePixelY);
            }
            else if (player == null) {
                Debug.WriteLine($"MapSystem: Player is NULL.");
            }

            Debug.WriteLine("MapSystem: Random map generated successfully.");
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
        public Vector2 GetRandomEmptyTile()
        {
            Vector2 randomTile;

            do
            {
                int x = random.Next(mapWidth);
                int y = random.Next(mapHeight);
                Sprite tile = Tilemap.GetTileAt(x, y);

                // Check if it's a floor tile (not an obstacle)
                if (tile.Texture.Name == "floor")
                {
                    randomTile = new Vector2(x, y);
                    break;
                }
            } while (true); // Keep looping until a valid tile is found

            return randomTile;
        }

        // Loads a map from a text file, parsing tile characters and setting textures accordingly.
        public void LoadMapFromFile(string filePath)
        {
            try
            {
                // Read all lines from the file
                string[] lines = System.IO.File.ReadAllLines(filePath);
                Debug.WriteLine($"MapSystem: Loaded map with " +
                    $"{lines.Length} lines and {lines[0].Length} " +
                    $"characters in first line.");

                // Validate map dimensions
                if (lines.Length != mapHeight || lines[0].Length != mapWidth)
                {
                    Debug.WriteLine("MapSystem Error: Map file dimensions do not match expected size.");
                    return;
                }

                // Loop through each line
                for (int y = 0; y < mapHeight; y++)
                {   // Loop throug each char
                    for (int x = 0; x < mapWidth; x++)
                    {
                        Sprite tile = Tilemap.GetTileAt(x, y);
                        char tileChar = lines[y][x];

                        switch (tileChar)
                        {
                            case 'F': tile.Texture = Tilemap.floorTexture; break;
                            case 'X': tile.Texture = Tilemap.obstacleTexture; break;
                            case 'S': tile.Texture = Tilemap.startTexture; break;
                            case 'E': tile.Texture = Tilemap.exitTexture; break;
                            default:
                                Debug.WriteLine($"Unknown tile '{tileChar}' at ({x}, {y})");
                                break;
                        }
                        Debug.WriteLine($"Tile at ({x}, {y}) set to {tileChar}");
                    }
                }
                Debug.WriteLine("Map successfully loaded from file.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading map: {ex.Message}");
            }
        }
    }
}
