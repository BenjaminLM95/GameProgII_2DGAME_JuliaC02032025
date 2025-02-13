using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.IO;
using static System.Net.WebRequestMethods;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{   
    internal class MapSystem : Component
    {
        GameManager _gameManager;
        public TileMap Tilemap { get; private set; }

        // ---------- VARIABLES ---------- //
        
        private Random random = new Random();
        private int mapHeight = 15; // map HEIGHT
        private int mapWidth = 25; // map WIDTH

        private int tilePixelX = 32; // tile sprites are 16x16
        private int tilePixelY = 32;

        private int obstacleDensity = 10; // __% of the map are obstacles

        // ---------- METHODS ---------- //

        public override void Start()
        {
            // Create TileMap and initialize it
            Tilemap = new TileMap();
            Tilemap.LoadTextures(Globals.Content);

            GenerateMap();
        }
        
        public void GenerateMap()
        {
            Tilemap.Initialize();

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Sprite tile = Tilemap.GetTileAt(x, y);

                    // Randomize the tile type
                    int randomValue = random.Next(100);  // Random value for randomness
                    if (randomValue < obstacleDensity)
                    {
                        tile.Texture = Globals.Content.Load<Texture2D>("obstacle");
                    }
                    else
                    {
                        tile.Texture = Globals.Content.Load<Texture2D>("floor");
                    }
                }
            }
            // manually place start/exit tiles
            Tilemap.GetTileAt(0, 0).Texture = Globals.Content.Load<Texture2D>("start");
            Tilemap.GetTileAt(mapWidth - 1, mapHeight - 1).Texture = Globals.Content.Load<Texture2D>("exit");
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Tilemap.Draw(spriteBatch);
        }

        ///<summary>
        /// Iterates over every tile in the structure file and loads its
        /// appearance and behavior. This method also validates that the
        /// file is well-formed with a player start point, exit, etc.
        /// </summary>
        /// <param name="filePath">
        /// A stream containing the tile data.
        /// </param>
        public void LoadMapFromFile(string filePath)
        {
            try
            {
                filePath = "C:\\MY FILES\\Programming\\Unity Projects NSCC\\GameProgII_2DGAME_JuliaC02032025\\MyMaps";
                string[] lines = System.IO.File.ReadAllLines(filePath);
                for (int y = 0; y < lines.Length; y++)
                {
                    for (int x = 0; x < lines[y].Length; x++)
                    {
                        //_map[x, y] = lines[y][x];
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading map: {ex.Message}");
            }
        }
    }
}
