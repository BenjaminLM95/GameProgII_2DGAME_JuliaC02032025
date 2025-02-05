using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class Player : Component
    {
        /// <summary>
        /// Responsible for handling player input, and updating the player's position with Draw()
        /// </summary>
       
        // REFERENCES
        // HealthSystem
        // Collision

        // VARIABLES
        // Get health properties from 
        // WASD input movement (Update?)
        // override Update() from component
        protected override void Draw() // overriding from Component
        {
            // draw player sprite from Sprite Texture2D - use InitializeSprite() ?
        }
        // METHODS TO USE - centralize these to Game1?
        // CheckCollisions() (method from collision component)
        // Die() (from HealthSystem)
    }
}
