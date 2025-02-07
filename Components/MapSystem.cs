using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
        
        // reference isCollider from Sprite - NO: assign colliders inside Sprite instead
        // needs to be able to randomize on load AND Load predefined maps from files

        // ---------- METHODS ---------- //
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
        private void DrawRandomMap(SpriteBatch spriteBatch)
        {
            // assign each tile a random pos using rnd variable
            if (_sprite == null) {
                return; }

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    string tileType = tileMap[x, y];
                    Texture2D texture = _sprite.InitializeSprite(tileType);

                    if(texture != null)
                    {
                        Vector2 position = new Vector2(x * texture.Width, y * texture.Height);
                        //spriteBatch.Draw(texture, position, Color.White);
                    }
                }
            }
        }
        private void LoadMapFromFile()
        {
            // reference 2D map project from last term
        }
    }
}
