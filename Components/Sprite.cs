using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class Sprite : Component
    {
        // ---------- VARIABLES ---------- //
        // need spritebatch?
        public bool isCollider { get; private set; }

        // make dictionary equating each sprite to a character(OR string NAME) like in 2D map project
        private Dictionary<string, Texture2D> sprites;

        // ---------- METHODS ---------- //

        // Initialize dictionary
        public Sprite()
        {
            sprites = new Dictionary<string, Texture2D>();
        }
        // Populate dictionary
        public void LoadSprites(ContentManager content)
        {
            // Loading textures
            Texture2D player = content.Load<Texture2D>("player");
            Texture2D enemy = content.Load<Texture2D>("enemy");
            // Loading map textures
            Texture2D mapFloor = content.Load<Texture2D>("mapFloor"); // walkable
            Texture2D mapObstacle = content.Load<Texture2D>("mapObstacle"); // non-walkable
            Texture2D mapStart = content.Load<Texture2D>("mapStart"); // spawnpoint
            Texture2D mapEnd = content.Load<Texture2D>("mapEnd"); // exit/next level

            // Add textures to dictionary
            sprites.Add("Player", player);
            sprites.Add("Enemy", enemy);
            sprites.Add("Floor", mapFloor);
            sprites.Add("Obstacle", mapObstacle);
            sprites.Add("Start", mapStart);
            sprites.Add("End", mapEnd);
        }
       
        public Texture2D InitializeSprite(string name)
        {
            // get all sprites from file names
            return sprites.TryGetValue(name, out Texture2D texture) ? texture : null;
        }
    }
}
