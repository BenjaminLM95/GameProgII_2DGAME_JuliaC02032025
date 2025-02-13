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
        /// Responsible for handling player input, and updating the player's Position with Draw()
        /// </summary>
  
        GameManager _gameManager;

        // ---------- VARIABLES ---------- //
        // Get/Use health properties from HealthSystem      
        private float speed = 5f;

        // CONSTRUCTOR
        public Player(Texture2D texture, Vector2 position) {
            sprite = Globals.Content.Load<Texture2D>("player");
        }

        // ---------- METHODS ---------- //       
        /// <summary>
        /// Reads player input and updates Position.
        /// </summary>
        public void ReadInput() 
        {            
            var KeyboardState = Keyboard.GetState();

            if (KeyboardState.IsKeyDown(Keys.W)) position.Y -= 1 * speed; // UP
            if (KeyboardState.IsKeyDown(Keys.A)) position.X -= 1 * speed; // LEFT
            if (KeyboardState.IsKeyDown(Keys.S)) position.Y += 1 * speed; // DOWN
            if (KeyboardState.IsKeyDown(Keys.D)) position.X += 1 * speed; // RIGHT           
        }

        /// <summary>
        /// Updates the player's state.
        /// </summary>
        public override void Update(float deltaTime)
        {           
            //base.Update(gameTime);
            ReadInput();
            //_collision?.CheckCollisions();
            //_sprite.Position = Position; // sprite null error in build !!!
            //_healthSystem.(); -- mmake CheckDamage() method??
        }

        /// <summary>
        /// Draws the player sprite
        /// </summary> 
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            _gameManager._sprite.Draw(spriteBatch);
        }
    }
}
