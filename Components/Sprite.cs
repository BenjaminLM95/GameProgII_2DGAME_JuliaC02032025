using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class Sprite : Component
    {
        // ---------- VARIABLES ---------- //       
        public Texture2D Texture { get; private set; }
        public Vector2 Position { get; set; } 
        public Vector2 Origin { get; protected set; }
        public bool isCollider { get; private set; }


        // ---------- METHODS ---------- //
        public Sprite(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
            Origin = new(Texture.Width / 2, Texture.Height / 2);
        }

        //public Sprite(string textureName, ContentManager content, Vector2 position)
        //{
         //   InitializeSprite(textureName, content);
          //  Position = position;
        //}

        /// <summary>
        /// Loads texture by name, content for Game1 Content.RootDirectory?
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        public void InitializeSprite(string name, ContentManager content)
        {
            //Texture = Game1.instance.Content.Load<Texture2D>(name);
            Texture = content.Load<Texture2D>(name);
        }

        /// <summary>
        /// Draws the sprite at it's position
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Texture != null)
            {
                spriteBatch.Draw(Texture, Position, Color.White);
            }
            base.Draw(spriteBatch);
        }
    }
}
