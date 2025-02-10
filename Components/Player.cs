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
        Sprite _sprite;

        // ---------- VARIABLES ---------- //
        // Get/Use health properties from HealthSystem      
        public Vector2 position = new Vector2(); // Vector2 position
        private float speed = 5f;

        // ---------- METHODS ---------- //       
        private void ReadInput() // WASD input movement (Update?)
        {
            if(Keyboard.GetState().IsKeyDown(Keys.W)) // UP
            {
                position.Y -= 1 * speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A)) // LEFT
            {
                position.X -= 1 * speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) // DOWN
            {
                position.Y += 1 * speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D)) // RIGHT
            {
                position.X += 1 * speed;
            }
        }
        
        // METHODS TO USE - centralize these to Game1?, use in Update()
        // CheckCollisions() (method from collision component)
        // Die() (from HealthSystem)
        private void Update()
        {
            ReadInput();
            _collision.CheckCollisions();
            //_healthSystem.(); -- mmake CheckDamage() method??
        }
    }
}
