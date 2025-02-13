using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class Sprite : Component
    {
        GameManager _gameManager;

        // ---------- VARIABLES ---------- //       
        //private Texture2D texture;
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; } 
        public bool isCollider { get; private set; }
        private int spriteScale = 2;


        // ---------- METHODS ---------- //
        public Sprite() { } // Default constructor 
        public Sprite(Texture2D texture, Vector2 position)
        {
            this.Texture = texture;
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

            Texture = Globals.Content.Load<Texture2D>(textureName);           
        }

        /// <summary>
        /// Draws the sprite at the GameObject's position.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Texture != null)
            {
                // Original size
                int width = Texture.Width;
                int height = Texture.Height;
                // Scale sprite
                Rectangle destinationRectangle = new Rectangle(
                    (int)GameObject.Position.X,
                    (int)GameObject.Position.Y,
                    width * spriteScale, height * spriteScale); // scaling

                //spriteBatch.Draw(Texture, Position, Color.White);
                spriteBatch.Draw(Texture, destinationRectangle, Color.White);
            }
        } 
    }
}
