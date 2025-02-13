using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class Sprite : Component
    {
        GameManager _gameManager;

        // ---------- VARIABLES ---------- //       
        private Texture2D texture;
        public Vector2 Position { get; set; } 
        public bool isCollider { get; private set; }


        // ---------- METHODS ---------- //
        public Sprite() { } // Default constructor 
        public Sprite(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.Position = position;
        }

        /// <summary>
        /// Loads a texture from the Content Manager.
        /// </summary>
        /// <param name="textureName"></param>
        public virtual void LoadSprite(string textureName)
        {
            if (Globals.Content == null)
            {
                throw new Exception("Globals.Content is not initialized! Ensure Game1.LoadContent() runs before calling LoadSprite().");
            }

            texture =Globals.Content.Load<Texture2D>(textureName);           
        }

        /// <summary>
        /// Draws the sprite at the GameObject's position.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
            {
                spriteBatch.Draw(texture, GameObject.Position, Color.White);
            }
        }
    }
}
