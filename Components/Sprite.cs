using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static System.Formats.Asn1.AsnWriter;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal class Sprite : Component
    {
        Globals globals;

        // ---------- VARIABLES ---------- //       

        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }

        private int fixedSize = 32;
        private float spriteScale = 1.0f; 


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
                Rectangle destinationRectangle = new Rectangle(
                (int)GameObject.Position.X,
                (int)GameObject.Position.Y,
                (int)(fixedSize * spriteScale),
                (int)(fixedSize * spriteScale));

                spriteBatch.Draw(
                Texture,              // The texture to draw
                destinationRectangle, // Destination rectangle (32x32 or scaled version)
                null,                 // Source rectangle (null = use the entire texture)
                Color.White,          // Color mask
                0f,                   // Rotation
                Vector2.Zero,         // Origin point
                SpriteEffects.None,   // No effects
                0f                    // Layer depth
                );
            }
        }
    }
}
