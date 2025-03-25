using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal class Items
    {
        public Items() { } // include item properties

        public void PlaceItems()
        {
            // find random empty tiles around the map
            // place items from list/array of possible items
        }

        public void UseItem()
        {
            // take input from player (button or mouse?)
            // use corresponding method
            // remove item from inventory
        }

        private void HealingPotion()
        {
            /*
                Restores HP when used
                Immediate effect
                Ends player's turn when used
            */
        }
        private void ScrollOfFireball()
        {
            /*
                Requires direction input after use
                Projectile travels instantly until collision 
                    (it travels and collides in the same turn it was used)
                Ends player's turn when used
            */
        }
        private void ScrollOfLightning()
        {
            /*
                Scroll of Lightning
                Damages all visible enemies
                Ends player's turn when used
            */
        }
        private void WarpScroll()
        {
            // sends player to a random empty tile
        }
    }
}