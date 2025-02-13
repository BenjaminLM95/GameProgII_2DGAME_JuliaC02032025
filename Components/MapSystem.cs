using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Net.WebRequestMethods;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class MapSystem : Component
    {
        /// <summary>
        /// Sets up a 10x15 tilemap of sprites taken from the 2D textures in Sprite. 
        /// Then creates an array of sprites in a random configuration.
        /// </summary>

        GameManager _gameManager;

        // ---------- VARIABLES ---------- //

        private const int mapHeight = 10; // map HEIGHT
        private const int mapWidth = 15; // map WIDTH
        private Tile[,] _tiles;
        private Random random = new();

        // Texures
        public Texture2D _floorTexture { get; private set; }
        public Texture2D _obstacleTexture { get; private set; }
        public Texture2D _startTexture { get; private set; }
        public Texture2D _exitTexture { get; private set; }

        internal class Tile
        {
            public Texture2D Texture { get; }
            public Vector2 Position { get; }

            public Tile(Texture2D texture, Vector2 position)
            {
                Texture = texture;
                Position = position;
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(Texture, Position, Color.White);
            }
        }

        // ---------- METHODS ---------- //

        public override void Start()
        {
            // Load textures from Global.Content
            _floorTexture = Globals.Content.Load<Texture2D>("floor");
            _obstacleTexture = Globals.Content.Load<Texture2D>("obstacle");
            _startTexture = Globals.Content.Load<Texture2D>("start");
            _exitTexture = Globals.Content.Load<Texture2D>("exit");

            // Debugging
            if (_floorTexture == null) Console.WriteLine("Floor texture is null!");
            if (_obstacleTexture == null) Console.WriteLine("Obstacle texture is null!");
            if (_startTexture == null) Console.WriteLine("Start texture is null!");
            if (_exitTexture == null) Console.WriteLine("Exit texture is null!");

            GenerateMap();
        }

        public void GenerateMap()
        {
            _tiles = new Tile[mapWidth, mapHeight];

            if (_tiles == null) // debug check
            {
                System.Console.WriteLine("Error: _tiles array is null. Map generation failed.");
                return;
            }

            // Fill map with floor tiles
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (_floorTexture == null)
                        Console.WriteLine("Error: Floor texture is null!");
                    _tiles[x, y] = new Tile(_floorTexture, new Vector2(x * 32, y * 32));
                }
            }

            // Place Start tile
            Point start = GetRandomEmptyTile();
            _tiles[start.X, start.Y] = new Tile(_startTexture, 
                new Vector2(start.X * 32, start.Y * 32));

            // Place Exit tile
            Point exit;
            do { exit = GetRandomEmptyTile(); } while (exit == start);
            _tiles[exit.X, exit.Y] = new Tile(_exitTexture, 
                new Vector2(exit.X * 32, exit.Y * 32)); // Ensure exit is not placed at the same Position as start

            // Place Obstacle tiles (20% of the map)
            int obstacleCount = (mapWidth * mapHeight) / 5;
            for (int i = 0; i < obstacleCount; i++)
            {
                Point obstaclePos = GetRandomEmptyTile();
                _tiles[obstaclePos.X, obstaclePos.Y] = 
                    new Tile(_obstacleTexture, 
                    new Vector2(obstaclePos.X * 32, obstaclePos.Y * 32));
            }
        } 

        ///<summary>
        /// Iterates over every tile in the structure file and loads its
        /// appearance and behavior. This method also validates that the
        /// file is well-formed with a player start point, exit, etc.
        /// </summary>
        /// <param name="fileStream">
        /// A stream containing the tile data.
        /// </param>
        private void LoadMapFromFile(Stream fileStream)
        {
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while(line != null)
                {
                    lines.Add(line);
                    if(line.Length != width) {
                        throw new Exception(String.Format
                            ("The length of line {0} is different from all preceeding lines.", 
                            lines.Count));
                    }
                    line = reader.ReadLine();
                }
            }
        }

        // Helper methods
        public bool IsValidTile(Point pos)
        {
            return pos.X >= 0 && pos.X < mapWidth && pos.Y >= 0 && pos.Y < mapHeight;
        }

        public Tile GetTileAt(Point pos)
        {
            if(!IsValidTile(pos)) return null;
            return _tiles[pos.X, pos.Y];
        }

        public Vector2 GetStartTilePosition()
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (_tiles[x,y].Texture == _startTexture)
                        return new Vector2(x, y);
                }
            }
            return Vector2.Zero;
        }

        private Point GetRandomEmptyTile()
        {
            Point pos;
            do
            {
                pos = new Point(random.Next(mapWidth), random.Next(mapHeight));
            } while (_tiles[pos.X, pos.Y] != null && _tiles[pos.X, pos.Y].Texture != _floorTexture);
            return pos;
        }

        /// <summary>
        /// Draws the map.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            Console.WriteLine("Map is being drawn...");
            foreach (var tile in _tiles)
            {
                if (tile == null)
                    Console.WriteLine("Null tile found!");
            }

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    Tile tile = _tiles[x, y];
                    if (tile != null)
                    {
                        tile.Draw(spriteBatch);  // Draw the tile at its specified position
                    }
                }
            }
        }
    }
}
