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
        private Tile _sprite;

        // ---------- VARIABLES ---------- //
        // Get/Use health properties from HealthSystem      
        public Vector2 Position { get; private set; } = new Vector2(); // Vector2 position
        private float speed = 5f;

        // CONSTRUCTOR
        public Player(ContentManager content)
        {
            //_sprite = new Tile("playerTexture", content, Position);
        }

        // ---------- METHODS ---------- //       
        /// <summary>
        /// Reads player input and updates position.
        /// </summary>
        private void ReadInput() 
        {
            Vector2 newPosition = Position; // make copy of current position
            var KeyboardState = Keyboard.GetState();

            if(KeyboardState.IsKeyDown(Keys.W)) newPosition.Y -= 1 * speed; // UP
            if (KeyboardState.IsKeyDown(Keys.A)) newPosition.X -= 1 * speed; // LEFT
            if (KeyboardState.IsKeyDown(Keys.S)) newPosition.Y += 1 * speed; // DOWN
            if (KeyboardState.IsKeyDown(Keys.D)) newPosition.X += 1 * speed; // RIGHT
            
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
