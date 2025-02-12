using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class Player : Component
    {
        /// <summary>
        /// Responsible for handling player input, and updating the player's position with Draw()
        /// </summary>

        // ---------- REFERENCES ---------- //
        // HealthSystem
        HealthSystem _healthSystem;
        // Collision
        Collision _collision;
        // sprite
        private Sprite _sprite;

        // ---------- VARIABLES ---------- //
        // Get/Use health properties from HealthSystem      
        public Vector2 Position { get; private set; } = new Vector2(); // Vector2 position
        private float speed = 5f;

        // CONSTRUCTOR
        public Player(ContentManager content)
        {
            _sprite = new Sprite("playerTexture", content, Position);
        }

        // ---------- METHODS ---------- //       
        /// <summary>
        /// Reads player input and updates position.
        /// </summary>
        private void ReadInput() 
        {
            Vector2 newPosition = Position; // make copy of current position

            if(Keyboard.GetState().IsKeyDown(Keys.W)) // UP
            {
                newPosition.Y -= 1 * speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A)) // LEFT
            {
                newPosition.X -= 1 * speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) // DOWN
            {
                newPosition.Y += 1 * speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D)) // RIGHT
            {
                newPosition.X += 1 * speed;
            }
            Position = newPosition; // reassign modified pos back to property
        }

        /// <summary>
        /// Updates the player's state.
        /// </summary>
        public override void Update()
        {
            ReadInput();
            _collision?.CheckCollisions();
            _sprite.Position = Position;
            //_healthSystem.(); -- mmake CheckDamage() method??
        }

        /// <summary>
        /// Draws the player sprite
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            _sprite.Draw(spriteBatch);
        }
    }
}
