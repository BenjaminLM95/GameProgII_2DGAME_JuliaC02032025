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
//Texture2D texture = Content.Load<Texture2D>("player");
// Load Map textures
//MapSystem map = new MapSystem(
//Content.Load<Texture2D>("floor"),
//Content.Load<Texture2D>("obstacle"),
//Content.Load<Texture2D>("start"),
//Content.Load<Texture2D>("exit")
//);
//Texture = Content.Load<Texture2D>("floor"); // test