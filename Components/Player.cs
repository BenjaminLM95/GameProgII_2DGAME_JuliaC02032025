using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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

        // override Update() from component
        protected override void Draw() // overriding from Component
        {
            // draw player sprite from Sprite Texture2D - use InitializeSprite() ?
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
