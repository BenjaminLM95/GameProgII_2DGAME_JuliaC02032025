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
        GameManager _gameManager;

        // ---------- VARIABLES ---------- //       
        public Texture2D Texture { get; private set; }
        public Vector2 Position { get; set; } 
        //public Vector2 Origin { get; protected set; }
        public bool isCollider { get; private set; }


        // ---------- METHODS ---------- //
        public Sprite(Texture2D texture, Vector2 position)
        {
            this.Texture = texture;
            this.Position = position;
            //Origin = new(this.Texture.Width / 2, this.Texture.Height / 2);
        }

        void LoadSprites()
        {
            //Texture2D texture = Content.Load<Texture2D>("player");
            // Load Map textures
            //MapSystem map = new MapSystem(
            //Content.Load<Texture2D>("floor"),
            //Content.Load<Texture2D>("obstacle"),
            //Content.Load<Texture2D>("start"),
            //Content.Load<Texture2D>("exit")
            //);
            //Texture = Content.Load<Texture2D>("floor"); // test
        }
        
        /// <summary>
        /// Loads Texture by name, content for Game1 Content.RootDirectory?
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        public void InitializeSprite(string name, ContentManager content)
        {
            //Texture = Game1.instance.Content.Load<Texture2D>(name);
            Texture = content.Load<Texture2D>(name);
        }

        /// <summary>
        /// Draws the sprite at it's Position
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
