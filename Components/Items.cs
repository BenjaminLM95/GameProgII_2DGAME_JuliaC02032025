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

        private void HealingPotion()
        {

        }
        private void ScrollOfFireball()
        {

        }
        private void ScrollOfLightning()
        {

        }
        private void CustomItem()
        {

        }
    }
}
/*
Required Items:
Healing Potion
Restores HP when used
Immediate effect
Ends player's turn when used

Scroll of Fireball
Requires direction input after use
Projectile travels instantly until collision (it travels and collides in the same turn it was used)
Ends player's turn when used

Scroll of Lightning
Damages all visible enemies
Ends player's turn when used

Custom Item
Unique utility effect
Different from existing items
 */