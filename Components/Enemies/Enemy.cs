using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGame_Julia_C02032025.Components.Enemies
{
    internal class Enemy : Component
    {
        private Globals globals;
        private HealthSystem healthSystem;
        private Pathfinding pathfinding;
        private TileMap tileMap;

        private Basic basicEnemy;
        private Advanced advancedEnemy;
        private Sprite enemySprite;

        // ---------- VARIABLES ---------- //
        private int minEnemyCount = 2;
        private int maxEnemyCount = 10;
        private List<Enemy> enemies = new List<Enemy>();

        private bool isStunned = false;
        public bool enemyMovedOntoPlayerTile { get; private set; }

        // make constructor for health, position, base variables

        // ---------- METHODS ---------- //

        public override void Start()
        {
            globals = Globals.Instance; // globals
            if (globals == null)
            {
                Debug.WriteLine("Player: globals is NULL!");
                return;
            }

            enemySprite = GameObject.GetComponent<Sprite>(); // sprite
            if (enemySprite == null)
            {
                Debug.WriteLine("Enemy: Sprite component is NULL!");
            }

            healthSystem = GameObject.GetComponent<HealthSystem>(); // healthSystem
            if (healthSystem == null)
            {
                healthSystem = GameObject.FindObjectOfType<HealthSystem>();
            }
        }
        public void Update()
        {
            if (isStunned) return; // Skip turn if stunned

            //Point enemyPosition = 
            //Player player = gameManager._player;

                //if (IsNextToPlayer(player))
                //{
                //    Debug.WriteLine("Enemy: attacking player");
                //    Attack(player);
                //}
            else
            {
                Debug.WriteLine("Enemy: moving towards player");
                //MoveTowardsPlayer(player);
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
            Vector2 randomTile = globals._mapSystem.GetRandomEmptyTile();

            if (randomTile != null)
            {
                for (int y = 0; y < 10; y++)
                {
                    for (int x = 0; x < 15; x++)
                    {
                        Sprite tile = tileMap.GetTileAt(x, y);
                        if (tile != null)
                        {
                            Debug.WriteLine($"Enemy: SpawnEnemy at position - {randomTile}");
                            tile.Texture = tileMap.enemyTexture;
                            //Globals.spriteBatch.Draw(tile.Texture, randomTile, Color.White);

                            Enemy newEnemy = new Enemy();
                            newEnemy.GameObject.Position = randomTile;
                            enemies.Add(newEnemy);
                        }
                    }
                }
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
                moveDirection.X = playerPos.X > enemyPos.X ? 32 : -32;
            }
            else
            {
                // Otherwise, move vertically
                moveDirection.Y = playerPos.Y > enemyPos.Y ? 32 : -32;
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

        private bool CheckForPlayer(Point enemyPosition)
        {
            Debug.WriteLine($"Enemy: checking for player at {enemyPosition.X}, {enemyPosition.Y}");
            if (tileMap == null) return false;

            Sprite targetTile = tileMap.GetTileAt(enemyPosition.X, enemyPosition.Y);

            if (targetTile != null && targetTile.Texture == tileMap.playerTexture) // changed from enemy to floor for testing debug
            {
                Debug.WriteLine($"Enemy: player found at {enemyPosition.X}, {enemyPosition.Y}!");
                enemyMovedOntoPlayerTile = true;
                return true;
            }
            return false;
        }
    }
}
