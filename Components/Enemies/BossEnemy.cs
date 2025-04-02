using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProgII_2DGame_Julia_C02032025.Components.Enemies
{
    class BossEnemy : Enemy
    {
        /*
         * Final Boss Requirements:
        More health than regular enemies.
        Handles their turn differently than regular enemies.
        Above their head they display an icon corresponding to what they will do on their next turn between these options:
            Nothing.
            Move towards the player.
            Shoot a projectile in the player's direction.
            Charge 3 tiles in the player's direction (Stopping if encounters the player or a wall, damaging the player if they encounter the player.
        If they are not aligned with the player when doing the charge or projectile attack, pick the closest direction they can shoot towards the player.
        Victory Condition:
            Clear game completion state when boss is defeated
            Option to restart game after victory
         */
    }
}
