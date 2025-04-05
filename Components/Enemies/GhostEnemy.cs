using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGame_Julia_C02032025.Components.Enemies
{
    internal class GhostEnemy : Enemy
    {
        public GhostEnemy() : base(EnemyType.Ghost)
        {
            config.SpriteName = "ghost";
            config.MaxHealth = 30;
            config.Damage = 10;
            config.MovementSpeed = 10f;
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

        public override void MoveTowardsPlayer(Player player, bool debug = false)
        {
            // basic movement logic - move one tile towards the player
            Vector2 playerPos = player.GameObject.Position;
            Vector2 enemyPos = GameObject.Position;

            // calculate direction to player
            Vector2 direction = playerPos - enemyPos;

            // normalize to get the primary direction
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                // move horizontally
                direction.Y = 0;
                direction.X = direction.X > 0 ? 32 : -32;
            }
            else
            {
                // move vertically
                direction.X = 0;
                direction.Y = direction.Y > 0 ? 32 : -32;
            }

            // calculate target position
            Vector2 targetPos = enemyPos + direction;
            Point targetTile = new Point((int)targetPos.X / 32, (int)targetPos.Y / 32);
            // no tile walkable check
            GameObject.Position = targetPos;
            hasMoved = true;
        }
    }
}
