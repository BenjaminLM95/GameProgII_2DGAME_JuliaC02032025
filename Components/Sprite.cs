using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class Sprite : Component
    {
        // VARIABLES
        // bool isCollider
        // need spritebatch?
        public bool isCollider { get; private set; }

        // assign all tiles from files here
        Texture2D player;
        Texture2D enemy;
        // MAP
        Texture2D mapFloor; // walkable
        Texture2D mapObstacle; // non-walkable
        Texture2D mapStart; // spawnpoint
        Texture2D mapEnd; // exit/next level
        // make dictionary equating each sprite to a character like in 2D map project
        private void InitializeSprite()
        {
            // get all sprites from file names
        }
    }
}
