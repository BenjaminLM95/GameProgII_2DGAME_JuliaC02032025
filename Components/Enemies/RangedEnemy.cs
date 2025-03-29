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
    internal class RangedEnemy : Enemy
    {
        private float attackCooldown = 2.0f; // Time between attacks
        private float timeSinceLastAttack = 0f;

        public RangedEnemy() : base(EnemyType.Archer) // Set default type (override if needed)
        {
            config.SpriteName = "archer"; // Use archer sprite
            config.MaxHealth = 40;
            config.Damage = 15;
            config.MovementSpeed = 0; // Archers do not move
        }

        public override void Update(float deltaTime)
        {
            timeSinceLastAttack += deltaTime;

            Player player = GameObject.FindObjectOfType<Player>();
            if (player == null) return;

            if (timeSinceLastAttack >= attackCooldown)
            {
                ShootProjectile(player);
                timeSinceLastAttack = 0f;
            }
        }

        private void ShootProjectile(Player player)
        {
            Vector2 direction = Vector2.Normalize(player.GameObject.Position - GameObject.Position);

            if (direction == Vector2.Zero) return; 

            GameObject projectileObject = new GameObject();
            Sprite enemyProjSprite = new Sprite();
            Projectile projectile = new Projectile(enemyProjSprite, direction, 150f, player, Globals.Instance._mapSystem.Tilemap);

            projectileObject.AddComponent(projectile);
            projectileObject.AddComponent(enemyProjSprite);
            enemyProjSprite.LoadSprite("player_fireballproj");

            projectileObject.Position = GameObject.Position;

            Globals.Instance._scene.AddGameObject(projectileObject);
            Debug.WriteLine("RangedEnemy: Fired a projectile at the player");
        }
    }
}

