using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGAME_JuliaC02032025.Components.Enemies
{
    internal class Enemy : Component
    {
        private Globals gameManager;
        private HealthSystem healthSystem;
        private Pathfinding pathfinding;

        private Basic basicEnemy;
        private Advanced advancedEnemy;

        // ---------- VARIABLES ---------- //
        private int minEnemyCount = 2;
        private int maxEnemyCount = 10;
        private List<Enemy> enemies = new List<Enemy>();

        private bool isStunned = false;
        public bool enemyMovedOntoPlayerTile { get; private set; }

        // ---------- METHODS ---------- //

        public override void Start()
        {
            healthSystem = GameObject.GetComponent<HealthSystem>();
            if (healthSystem == null)
            {
                healthSystem = GameObject.FindObjectOfType<HealthSystem>();
            }
        }
        public void Update()
        {
            if (isStunned) return; // Skip turn if stunned

            Player player = gameManager._player;

            if (IsNextToPlayer(player))
            {
                Attack(player);
            }
            else
            {
                MoveTowardsPlayer(player);
            }
        }

        // Spawning
        public void SpawnEnemies(int level)
        {
            int enemyCount = Math.Min(level, maxEnemyCount); // Level-based scaling
            enemies.Clear();

            for (int i = 0; i < enemyCount; i++)
            {
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            // Get a random floor tile
            Vector2 randomTile = gameManager._mapSystem.GetRandomEmptyTile();

            if (randomTile != null)
            {
                Enemy newEnemy = new Enemy();
                newEnemy.GameObject.Position = randomTile;
                enemies.Add(newEnemy);
            }
        }
        // Tilemap Movement
        public void MoveTowardsPlayer(Player player)
        {
            Vector2 playerPos = player.GameObject.Position;
            Vector2 enemyPos = GameObject.Position;
            Vector2 moveDirection = Vector2.Zero;

            if (Math.Abs(playerPos.X - enemyPos.X) > Math.Abs(playerPos.Y - enemyPos.Y))
            {
                // Move horizontally first if player is further in X direction
                moveDirection.X = (playerPos.X > enemyPos.X) ? 32 : -32;
            }
            else
            {
                // Otherwise, move vertically
                moveDirection.Y = (playerPos.Y > enemyPos.Y) ? 32 : -32;
            }

            GameObject.Position += moveDirection;
        }

        // Combat
        public void Attack(Player player)
        {
            if (player != null)
            {
                player.GameObject.GetComponent<HealthSystem>()?.TakeDamage(10);
            }
        }
        public void TakeDamage(int damage)
        {
            healthSystem.TakeDamage(damage);
            isStunned = true;
        }

        public void RecoverFromStun()
        {
            isStunned = false;
        }

        private bool IsNextToPlayer(Player player) // check if within 1 tile (32 x 32)
        {
            Vector2 playerPos = player.GameObject.Position;
            Vector2 enemyPos = GameObject.Position;

            return (Math.Abs(playerPos.X - enemyPos.X) == 32 && playerPos.Y == enemyPos.Y) ||
                   (Math.Abs(playerPos.Y - enemyPos.Y) == 32 && playerPos.X == enemyPos.X);
        }
    }
}
