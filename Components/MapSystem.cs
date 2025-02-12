using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class MapSystem : Component
    {
        /// <summary>
        /// Sets up a 10x15 tilemap of sprites taken from the 2D textures in Sprite. 
        /// Then creates an array of sprites in a random configuration.
        /// </summary>

        // ---------- REFERENCES ---------- //
        private Sprite _sprite;

        // ---------- VARIABLES ---------- //
        private const int mapHeight = 10; // map HEIGHT
        private const int mapWidth = 15; // map WIDTH

        private string[,] tileMap = new string[mapHeight, mapWidth]; 
        Random rnd = new Random();


        // ---------- METHODS ---------- //
        /// <summary>
        /// Create a 10x15 area and fill it with floor tiles, 
        /// then find random positions to place start, end, and obstacle tiles
        /// </summary>
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

            if (_sprite == null) {
                return; }

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    string tileType = tileMap[x, y];
                    Texture2D texture = _sprite.InitializeSprite(tileType); // method needs (string name, ContentManager content)

                    if (texture != null) {
                        Vector2 position = new Vector2(x * texture.Width, y * texture.Height);
                        spriteBatch.Draw(texture, position, Color.White);
                    }
                }
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
            // reference 2D map project from last term
            // find something like a base directory from this computer, make file in this project for maps
            // like 2D RPG, chars = tiles
        }
    }
}
