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
                Vector2 shootDirection;
                if (HasLineOfSightToPlayer(player, out shootDirection))
                {
                    ShootProjectile(shootDirection, player);
                    timeSinceLastAttack = 0f;
                }
                else
                {
                    MoveTowardsPlayer(player);
                }
            }
        }

        private bool HasLineOfSightToPlayer(Player player, out Vector2 direction)
        {
            direction = Vector2.Zero;

            Vector2 playerPos = player.GameObject.Position;
            Vector2 enemyPos = GameObject.Position;

            // only straight lines of sight (same X or same Y)
            if (Math.Abs(playerPos.X - enemyPos.X) < 1f)
            {
                direction = playerPos.Y > enemyPos.Y ? Vector2.UnitY : -Vector2.UnitY;
            }
            else if (Math.Abs(playerPos.Y - enemyPos.Y) < 1f)
            {
                direction = playerPos.X > enemyPos.X ? Vector2.UnitX : -Vector2.UnitX;
            }
            else {
                return false; // no diagonal
            }

            Vector2 checkPos = enemyPos + direction;
            while (Vector2.DistanceSquared(checkPos, playerPos) >= 1f)
            {
                // convert world position to tile coordinate
                Point tileToCheck = new Point((int)(checkPos.X / 32), (int)(checkPos.Y / 32));

                if (!IsTileWalkable(tileToCheck))
                {
                    return false; // obstacle blocks line of sight
                }
                checkPos += direction;
            }

            return true;
        }

        private void ShootProjectile(Vector2 direction, Player player)
        {
            if (direction == Vector2.Zero) return; 

            // creating a projectile gameobject for enemies
            GameObject projectileObject = new GameObject();
            Sprite enemyProjSprite = new Sprite();
            Projectile projectile = new Projectile(
                enemyProjSprite, // enemy projectile sprite
                direction,       // direction the proj is going
                150f,            // proj speed
                player,          // ref. to the player
                Globals.Instance._mapSystem.Tilemap, // ref. to the tilemap
                ProjectileSource.Enemy);             // projectile's source is an enemy

            projectileObject.AddComponent(projectile);
            projectileObject.AddComponent(enemyProjSprite);
            enemyProjSprite.LoadSprite("archer_proj");

            projectileObject.Position = GameObject.Position;

            Globals.Instance._scene.AddGameObject(projectileObject);
            Debug.WriteLine("RangedEnemy: Fired a projectile at the player");
        }
    }
}

// needs to move if they dont have a clear shot / sight line of the player.
// either shoot or move on their turn
// projectiles cannot go diagonally