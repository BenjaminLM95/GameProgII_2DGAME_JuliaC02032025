using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
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
        private Tile[,] _tiles;

        // ---------- VARIABLES ---------- //
        //private Point mapTileSize = new(10, 15);
        private const int mapHeight = 10; // map HEIGHT
        private const int mapWidth = 15; // map WIDTH
        Random random = new();

        private Texture2D _floorTexture;
        private Texture2D _obstacleTexture;
        private Texture2D _startTexture;
        private Texture2D _exitTexture;

        public MapSystem(Texture2D floor, Texture2D obstacle, Texture2D start, Texture2D exit, Vector2 position) // initialize map area
        {
            _floorTexture = floor;
            _obstacleTexture = obstacle;
            _startTexture = start;
            _exitTexture = exit;
            position = position;

            GenerateMap();
        }

        // ---------- METHODS ---------- //

        private void GenerateMap()
        {
            _tiles = new Tile[mapWidth, mapHeight];

            // Fill map with floor tiles
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    _tiles[x, y] = new Tile(_floorTexture, 
                        new Vector2(x * _floorTexture.Width, y * _floorTexture.Height));
                }
            }

            // Place Start tile
            Point start = GetRandomEmptyTile();
            _tiles[start.X, start.Y] = new Tile(_startTexture, 
                new Vector2(start.X * _startTexture.Width, start.Y * _startTexture.Height));

            // Place Exit tile
            Point exit;
            do
            {
                exit = GetRandomEmptyTile();
            } while (exit == start); // Ensure exit is not placed at the same Position as start

            _tiles[exit.X, exit.Y] = new Tile(_exitTexture, 
                new Vector2(exit.X * _exitTexture.Width, exit.Y * _exitTexture.Height));

            // Place Obstacle tiles (20% of the map)
            int obstacleCount = (mapWidth * mapHeight) / 5;
            for (int i = 0; i < obstacleCount; i++)
            {
                Point obstaclePos = GetRandomEmptyTile();
                _tiles[obstaclePos.X, obstaclePos.Y] = 
                    new Tile(_obstacleTexture, 
                    new Vector2(obstaclePos.X * _obstacleTexture.Width, 
                    obstaclePos.Y * _obstacleTexture.Height));
            }
        }

        private Point GetRandomEmptyTile()
        {
            Point pos;
            do
            {
                pos = new Point(random.Next(mapWidth), random.Next(mapHeight));
            } while (_tiles[pos.X, pos.Y].Texture != _floorTexture); // Ensure only replacing a floor tile
            return pos;
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    _tiles[x, y].Draw(spriteBatch);
                }
            }
        }

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
        /// <summary>
        /// Create a 10x15 area and fill it with floor tiles, 
        /// then find random positions to place start, end, and obstacle tiles
        /// </summary>
        /*
        private void RandomizeMap()
        {
            // double nested for loop using map Height/Width
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    tileMap[x, y] = "Floor"; // All tiles are initially floor
                }
            }
            // Place START tile
            int startX = rnd.Next(0, mapWidth);
            int startY = rnd.Next(0, mapHeight);
            tileMap[startY, startX] = "Start";

            // Place END tile
            int endX = rnd.Next(0, mapWidth);
            int endY = rnd.Next(0, mapHeight);
            tileMap[startY, startX] = "End";

            // Place obstacle tiles randomly
            int obstacleCount = (mapWidth * mapHeight) / 5; // 20% of tiles are obstacles
            for (int i = 0; i < obstacleCount; i++)
            {
                int obsX, obsY;
                do {
                    obsX = rnd.Next(mapWidth);
                    obsY = rnd.Next(mapHeight);
                } while (tileMap[obsY, obsX] != "Floor"); // Only place on floor tiles

                tileMap[obsY, obsX] = "Obstacle";
            }
        }
        /// <summary>
        /// Draw the tiles at their random positions
        /// </summary>
        /// <param name="spriteBatch"></param>
        private void DrawRandomMap(SpriteBatch spriteBatch)
        {
            // assign each tile a random pos using rnd variable
            // get sprite positions from RandomizeMap()

            if (_tiles == null) {
                return; }

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    string tileType = tileMap[x, y];
                    Texture2D Texture = _tiles.InitializeSprite(tileType); // method needs (string name, ContentManager content)

                    if (Texture != null) {
                        Vector2 Position = new Vector2(x * Texture.Width, y * Texture.Height);
                        spriteBatch.Draw(Texture, Position, Color.White);
                    }
                }
            }
        }
        */


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
            // reference 2D map project from last term
            // find something like a base directory from this computer, make file in this project for maps
            // like 2D RPG, chars = tiles
        }
    }
}
//Texture2D texture = Content.Load<Texture2D>("player");
// Load Map textures
//MapSystem map = new MapSystem(
//Content.Load<Texture2D>("floor"),
//Content.Load<Texture2D>("obstacle"),
//Content.Load<Texture2D>("start"),
//Content.Load<Texture2D>("exit")
//);
//Texture = Content.Load<Texture2D>("floor"); // test