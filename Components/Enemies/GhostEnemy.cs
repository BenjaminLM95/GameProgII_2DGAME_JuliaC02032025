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
        Globals globals;
        TileMap tileMap;
        Pathfinding pathfinding;
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

            // get the position of the player
            Vector2 targetPosition = player.GameObject.Position;
            Vector2 direction = targetPosition - GameObject.Position;

            // normalize the direction to ensure consistent speed
            if (direction.Length() > 0)
            {
                direction.Normalize();
            }

            // move the ghost towards the player by a certain speed
            GameObject.Position += direction * config.MovementSpeed * deltaTime;
        }
    }
}
