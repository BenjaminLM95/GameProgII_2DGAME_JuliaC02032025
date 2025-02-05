using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class MapSystem : Component
    {
        /// <summary>
        /// Sets up a 10x15 tilemap of sprites taken from the 2D textures in Sprite. 
        /// Then creates an array of sprites in a random configuration.
        /// </summary>
       
        // VARIABLES
        // int mapHeight
        // int mapWidth
        // Vector2 tilePosition
        Vector2 tilePosition = new Vector2();
        // reference isCollider from Sprite

        // reference Texture2D of sprite & dictionary from Sprite
        // set tilePosition
        // have array/list of sprites that are drawn into 10x15 map
        // needs to be able to randomize on load AND Load predefined maps from files
        private void RandomizeMap()
        {
            // reference 2D map project from last term
            // for each tile in the array, generate a random Vector2 position within the 10x15 area.
            // draw each tile in sequence
        }
        private void DrawRandomMap()
        {

        }
        private void LoadMapFromFile()
        {
            // reference 2D map project from last term
        }
    }
}
