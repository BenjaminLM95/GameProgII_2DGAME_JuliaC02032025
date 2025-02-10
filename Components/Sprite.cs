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
        public bool isCollider { get; private set; }
        public string spriteName = "";
        protected Texture2D currentTexture;

        // ---------- METHODS ---------- //
        // Initialize dictionary
        public Sprite()
        {

        }

        /// <summary>
        /// Load Sprites from imported 2D textures, then add them to the sprites dictionary that takes
        /// a string name and a Texture2D
        /// </summary>
        /// <param name="content"></param>
        // Populate dictionary
        public void LoadSprites(ContentManager content)
        {
            // Loading textures
            //Texture2D player = content.Load<Texture2D>("1");
            //Texture2D enemy = content.Load<Texture2D>("enemy");
            // Loading map textures
            //Texture2D mapFloor = content.Load<Texture2D>("floor"); // walkable
            //Texture2D mapObstacle = content.Load<Texture2D>("obstacle"); // non-walkable
            //Texture2D mapStart = content.Load<Texture2D>("start"); // spawnpoint
            //Texture2D mapEnd = content.Load<Texture2D>("end"); // exit/next level

            // Add textures to dictionary (can simplify by taking this out?)
            //sprites.Add("Player", player);
            //sprites.Add("Enemy", enemy);
            //sprites.Add("Floor", mapFloor);
            //sprites.Add("Obstacle", mapObstacle);
            //sprites.Add("Start", mapStart);
            //sprites.Add("End", mapEnd);
        }

        public Texture2D InitializeSprite(string name)
        {
            currentTexture = Game1.instance.Content.Load<Texture2D>(name);
            return currentTexture;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            //spriteBatch.Draw(currentTexture,)
        }
    }
}
