using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGame_Julia_C02032025.Components.Enemies
{
    internal class BasicEnemy : Enemy
    {
        public BasicEnemy() : base(EnemyType.Slime) // Set slime type
        {
            config.SpriteName = "enemy"; 
            config.MaxHealth = 30;
            config.Damage = 5;
            config.MovementSpeed = 1; 
        }
        public override void Update(float deltaTime)
        {
            Player player = GameObject.FindObjectOfType<Player>();
            if (player == null) return;

            if (IsNextToPlayer(player))
            {
                Attack(player);
            }
            else
            {
                MoveTowardsPlayer(player, debug: true);
            }
        }
        public override void Attack(Player player)
        {
            player.GameObject.GetComponent<HealthSystem>()?.ModifyHealth(-config.Damage);
        }
        public override void MoveTowardsPlayer(Player player, bool debug = false)
        {
            base.MoveTowardsPlayer((Player)player, debug);
        }
    }
}
