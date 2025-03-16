using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using static System.Net.WebRequestMethods;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{   
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

        private int obstacleDensity = 10; // __% of the map are obstacles

        List<Vector2> emptyTiles = new List<Vector2>();

        // ---------- METHODS ---------- //

        public override void Start()
        {
            // Create TileMap and initialize it
            Tilemap = new TileMap();
            Tilemap.LoadTextures(Globals.content);
            Tilemap.Initialize();

            // ---!!!--- MAP GENERATION ---!!!--- // <switch here

            GenerateMap();  // Random map generation

            // Load map from.txt file
            //LoadMapFromFile("C:\\MY FILES\\Programming\\Unity Projects NSCC\\" + 
            //"GameProgII_2DGAME_JuliaC02032025\\MyMaps\\Map1.txt");
            //LoadMapFromFile("C:\\Users\\W0517383\\Documents\\GitHub\\" +
            //  "GameProgII_2DGAME_JuliaC02032025\\MyMaps\\Map1.txt");
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
                    tile.Texture = Globals.content.Load<Texture2D>("floor");

                    if (CanPlaceObstacle(x, y))
                    {
                        // Randomly determine the size of the obstacle (between 2x2 and 4x4, including combinations like 2x3)
                        int obstacleWidth = random.Next(2, 5); // 2, 3, or 4 width
                        int obstacleHeight = random.Next(2, 5); // 2, 3, or 4 height

                        // Place the obstacle if there's enough space
                        if (CanFitObstacle(x, y, obstacleWidth, obstacleHeight))
                        {
                            // Mark the area as occupied by an obstacle
                            PlaceObstacle(x, y, obstacleWidth, obstacleHeight);
                            tile.Texture = Globals.content.Load<Texture2D>("obstacle"); // Mark as obstacle
                        }
                    }

                    /*
                    Sprite tile = Tilemap.GetTileAt(x, y);

                    // Implement wall rule

                    // Randomize the tile type
                    int randomValue = random.Next(100);
                    if (randomValue < obstacleDensity)
                    {
                        tile.Texture = Globals.content.Load<Texture2D>("obstacle"); 
                        // change this to obstacle rules
                    }
                    else
                    {
                        tile.Texture = Globals.content.Load<Texture2D>("floor");
                        // add to list of empty tiles to be referenced in GetRandomEmptyTile()
                        //emptyTiles.Add(reference Vector2 position);
                    }
                    */
                }
            }

            // Find random positions for start and exit tiles
            Vector2 startTile = GetRandomEmptyTile();
            Vector2 exitTile;

            // Keep finding a different exit tile until it's not the same as the start
            do {
                exitTile = GetRandomEmptyTile();
            } while (exitTile == startTile);

            // Set the start and exit tiles
            Tilemap.GetTileAt((int)startTile.X, (int)startTile.Y).Texture = Globals.content.Load<Texture2D>("start");
            Tilemap.GetTileAt((int)exitTile.X, (int)exitTile.Y).Texture = Globals.content.Load<Texture2D>("exit");
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Tilemap.Draw(spriteBatch);
        }

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
                    tile.Texture = Globals.content.Load<Texture2D>("obstacle"); // Change to obstacle texture
                }
            }
        }


        // Finds a random empty tile (not an obstacle) on the map.
        private Vector2 GetRandomEmptyTile()
        {
            Vector2 randomTile;

            do {
                int x = random.Next(mapWidth);
                int y = random.Next(mapHeight);
                Sprite tile = Tilemap.GetTileAt(x, y);

                // Check if it's a floor tile (not an obstacle)
                if (tile.Texture.Name == "floor") {
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
                Debug.WriteLine($"Loaded map with " +
                    $"{lines.Length} lines and {lines[0].Length} " +
                    $"characters in first line.");

                // Validate map dimensions
                if (lines.Length != mapHeight || lines[0].Length != mapWidth)
                {
                    Debug.WriteLine("Error: Map file dimensions do not match expected size.");
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
