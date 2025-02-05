using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class Collision : Component
    {
        // VARIABLES
        // rectangle for hitbox?
        // bool isCollided
        private bool isCollided;
        // check for Exit tile in Sprite - if collided load next map

        // METHODS
        // CheckCollisions() - make Vector2 inst of void? or BOOL?
        public void CheckCollisions()
        {
            // non-walkable/exit/start make rectangle around it
            // check for player sprite contact
        }
        // method for OnCollisionEnter()
        public void OnCollisionEnter()
        {
            // if its non-walkable make it so the player cant move on it
            // if Exit load next map from ref. MapSystem
            // if Start clear player tile then transform.position to start tile of next map
        }
    }
}
