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

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    // enum to identify the source of the projectile
    public enum ProjectileSource
    {
        Player,
        Enemy
    }

    internal class Projectile : Component
    {
        // ---------- REFERENCES ---------- //
        private Sprite projSprite;
        private Vector2 direction;
        private float speed;
        private Player player;
        private TileMap tileMap;
        private float lifetime = 3.0f; // Max lifetime in seconds
        private float elapsedTime = 0;
        private int damage = 30; // Projectile damage amount
        private int enemyDamage = 10; // Enemy Projectile damage amount
        private ProjectileSource source; // projectile is coming from an enemy or a player

        // ---------- CONSTRUCTOR ---------- //
        public Projectile( // constructor used when creating Projectile instance on gameobject
            Sprite texture, 
            Vector2 direction, 
            float speed, 
            Player player, 
            TileMap tileMap, 
            ProjectileSource source = ProjectileSource.Player)
        {
            this.projSprite = texture;
            this.direction = direction;
            this.speed = speed;
            this.player = player;
            this.tileMap = tileMap;
            this.source = source;
        }

        // ---------- METHODS ---------- //
        public override void Update(float deltaTime)
        {
            // move the projectile
            GameObject.Position += direction * speed * deltaTime;

            // check for collisions with walls/obstacles
            if (!IsValidTile(GameObject.Position, tileMap))
            {
                // hit a wall, destroy the projectile
                Globals.Instance._scene.RemoveGameObject(GameObject);
                Debug.WriteLine("Projectile: Fireball hit a wall and was destroyed");
                return;
            }

            // check for collisions with enemies
            if (source == ProjectileSource.Player)
            {
                CheckEnemyCollision();
            }
            else // if the ProjectileSource is an Enemy
            {
                CheckPlayerCollision();
            }

            // update lifetime and destroy if too old
            elapsedTime += deltaTime;
            if (elapsedTime >= lifetime)
            {
                Globals.Instance._scene.RemoveGameObject(GameObject);
                Debug.WriteLine("Projectile: Fireball expired after reaching maximum lifetime");
            }
        }

        // ---------- Player Projectiles ---------- //
        private void CheckEnemyCollision()
        {
            // get all enemies
            Enemy enemyManager = GameObject.FindObjectOfType<Enemy>();
            if (enemyManager != null)
            {
                List<Enemy> enemies = enemyManager.GetEnemies();

                foreach (Enemy enemy in enemies)
                {
                    // collision check
                    float distance = Vector2.Distance(GameObject.Position, enemy.GameObject.Position);
                    if (distance < 32) // tile size 32
                    {
                        // hit an enemy & apply damage
                        HealthSystem enemyHealth = enemy.GameObject.GetComponent<HealthSystem>();
                        if (enemyHealth != null)
                        {
                            enemyHealth.ModifyHealth(-damage);
                        }

                        // destroy the projectile after hitting an enemy
                        Globals.Instance._scene.RemoveGameObject(GameObject);
                        return;
                    }
                }
            }
        }

        // ---------- Enemy Projectiles ---------- //
        private void CheckPlayerCollision()
        {
            // Check collision with player
            if (player != null)
            {
                float distance = Vector2.Distance(GameObject.Position, player.GameObject.Position);
                if (distance < 32) // tile size 32
                {
                    // hit the player & apply damage
                    HealthSystem playerHealth = player.GameObject.GetComponent<HealthSystem>();
                    if (playerHealth != null)
                    {
                        playerHealth.ModifyHealth(-enemyDamage); // damage enemy projectile does to player
                    }
                    // destroy the projectile after hitting the player
                    Globals.Instance._scene.RemoveGameObject(GameObject);
                }
            }
        }

        private bool IsValidTile(Vector2 position, TileMap tileMap)
        {
            int x = (int)position.X / 32;
            int y = (int)position.Y / 32;
            Sprite tile = tileMap.GetTileAt(x, y);
            return tile != null && tile.Texture == tileMap.floorTexture;
        }
    }
}
