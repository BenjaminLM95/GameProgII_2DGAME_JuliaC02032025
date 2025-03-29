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
    internal class Projectile : Component
    {
        private Sprite projSprite;
        private Vector2 direction;
        private float speed;
        private Player player;
        private TileMap tileMap;
        private float lifetime = 3.0f; // Max lifetime in seconds
        private float elapsedTime = 0;
        private int damage = 25; // Projectile damage amount

        public Projectile(Sprite texture, Vector2 direction, float speed, Player player, TileMap tileMap)
        {
            this.projSprite = texture;
            this.direction = direction;
            this.speed = speed;
            this.player = player;
            this.tileMap = tileMap;
        }

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
            CheckEnemyCollision();

            // update lifetime and destroy if too old
            elapsedTime += deltaTime;
            if (elapsedTime >= lifetime)
            {
                Globals.Instance._scene.RemoveGameObject(GameObject);
                Debug.WriteLine("Projectile: Fireball expired after reaching maximum lifetime");
            }
        }

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
                            Debug.WriteLine($"Projectile: projectile hit enemy for {damage} damage");
                        }

                        // destroy the projectile after hitting an enemy
                        Globals.Instance._scene.RemoveGameObject(GameObject);
                        return;
                    }
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
