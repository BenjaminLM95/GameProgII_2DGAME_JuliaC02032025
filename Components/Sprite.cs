using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal class Sprite : Component
    {
        Globals globals;

        // ---------- VARIABLES ---------- //       

        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }

        private int spriteScale = 2;


        // ---------- METHODS ---------- //
        public Sprite() { } // Default constructor 
        public Sprite(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
        }

        // Loads a texture from the Content Manager.
        public virtual void LoadSprite(string textureName)
        {
            if (Globals.content == null)
            {
                throw new Exception("Globals.Content is not initialized! Ensure Game1.LoadContent() runs before calling LoadSprite().");
            }

            Texture = Globals.content.Load<Texture2D>(textureName);
        }

        // Draws the sprite at the GameObject's position.
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
                Globals.spriteBatch.Draw(Texture, destinationRectangle, Color.White);
            }
        }
    }
}
