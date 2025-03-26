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
        private static List<Enemy> _enemies = new List<Enemy>();

        private bool isStunned = false;
        public bool enemyMovedOntoPlayerTile { get; private set; }

        public Enemy()
        {
            globals = Globals.Instance;
            // Add this enemy instance to the static list when created
            _enemies.Add(this);
        }

        // ---------- METHODS ---------- //

        public override void Start()
        {
            globals = globals ?? Globals.Instance; // globals
            if (globals == null)
            {
                Debug.WriteLine("Enemy: globals is NULL!");
                throw new InvalidOperationException("Globals instance could not be initialized");
            }

            enemySprite = GameObject.GetComponent<Sprite>(); // sprite
            if (enemySprite == null)
            {
                Debug.WriteLine("Enemy: Sprite component is NULL!");
            }

            healthSystem = GameObject.GetComponent<HealthSystem>() ?? GameObject.FindObjectOfType<HealthSystem>();
            tileMap = globals._mapSystem?.Tilemap;
            pathfinding = GameObject.GetComponent<Pathfinding>();
        }
        public void Update()
        {
            if (isStunned) return; // Skip turn if stunned

            Player player = GameObject.FindObjectOfType<Player>();
            if (player == null) return;

            if (IsNextToPlayer(player))
            {
                Debug.WriteLine("Enemy: Attacking player");
                Attack(player);
                globals._combat.AdvanceToNextTurn();
            }
            else
            {
                Debug.WriteLine("Enemy: moving towards player");
                MoveTowardsPlayer(player);
                globals._combat.AdvanceToNextTurn();
            }
        }

        public bool IsNextToPlayer(Player player)
        {
            Debug.WriteLine("Enemy: checking if enemy is next to player");
            Vector2 playerPos = player.GameObject.Position;
            Vector2 enemyPos = GameObject.Position;
            return Math.Abs(playerPos.X - enemyPos.X) <= 32 && Math.Abs(playerPos.Y - enemyPos.Y) <= 32;
        }

        // Spawning
        public void SpawnEnemies(int level)
        {
            // Clear existing enemies
            _enemies.Clear();

            // Calculate number of enemies based on level
            int enemyCount = Math.Clamp(level, minEnemyCount, maxEnemyCount);

            Debug.WriteLine($"Enemy: Spawning {enemyCount} enemies for level {level}");

            for (int i = 0; i < enemyCount; i++)
            {
                SpawnEnemy();
            }
        }
        // move the gameobject-component logic back to Game1, this may fix the turn cycling issue
        private void SpawnEnemy()
        {
            if (globals == null)
            {
                globals = Globals.Instance;
            }
            if (globals._mapSystem == null)
            {
                Debug.WriteLine("Enemy: Cannot spawn - MapSystem is NULL!");
                return;
            }
            // Get a random floor tile
            Vector2 randomTile = globals._mapSystem.GetRandomEmptyTile();

            if (randomTile == new Vector2(-1, -1))
            {
                Debug.WriteLine("Enemy: No valid floor tile found for spawning.");
                return;
            }
            // Create new enemy game object
            GameObject enemyObject = new GameObject();
            Enemy newEnemy = new Enemy();
            Sprite enemySprite = new Sprite();
            Pathfinding enemyPathfinding = new Pathfinding();
            HealthSystem enemyHealth = new HealthSystem();

            // Add components to enemy game object
            enemyObject.AddComponent(newEnemy);
            enemyObject.AddComponent(enemySprite);
            enemyObject.AddComponent(enemyPathfinding);
            enemyObject.AddComponent(enemyHealth);

            // Load enemy sprite
            enemySprite.LoadSprite("enemy");

            // Set enemy position
            enemyObject.Position = randomTile;

            // Add to scene
            Globals.Instance._scene.AddGameObject(enemyObject);

            Debug.WriteLine($"Enemy: Spawned at position - {randomTile}");
            //Debug.WriteLine($"Enemy: SpawnEnemy at position - {randomTile}");

            //Enemy newEnemy = new Enemy();
            //newEnemy.GameObject.Position = randomTile;
            //_enemies.Add(newEnemy);
        }

        // Tilemap Movement
        public void MoveTowardsPlayer(Player player)
        {
            pathfinding = GameObject.GetComponent<Pathfinding>();
            
            Vector2 playerPos = player.GameObject.Position / 32;
            Vector2 enemyPos = GameObject.Position / 32;

            Point enemyPoint = new Point((int)enemyPos.X, (int)enemyPos.Y);
            Point playerPoint = new Point((int)playerPos.X, (int)playerPos.Y);

            Debug.WriteLine($"Enemy: position: {enemyPoint}, Player position: {playerPoint}");

            List<Point> path = pathfinding.FindPath(enemyPoint, playerPoint);

            // Correct null and empty path checking
            if (path == null || path.Count <= 1)
            {   // !!! getting this message| FIX!!!
                Debug.WriteLine($"Enemy: path finding failed. Path is {(path == null ? "null" : $"too short (count: {path.Count}")}");
                return;
            }

            // Move to the next tile in the path (path[1] is the next step)
            Point nextTile = path[1];
            Vector2 newPosition = new Vector2(nextTile.X * 32, nextTile.Y * 32);

            // Check if the tile is walkable before moving
            Sprite targetTile = tileMap.GetTileAt(enemyPoint.X, enemyPoint.Y);
            if (targetTile.Texture == tileMap.floorTexture)
            {
                GameObject.Position = newPosition;
                Debug.WriteLine($"Enemy moved to {newPosition}");
            }
            else {
                Debug.WriteLine("Enemy: Next tile is not walkable!");
            }
        }
        public List<Enemy> GetEnemies() 
        {
            _enemies.RemoveAll(e => e == null); // Remove any null enemies from the list
            return _enemies;
        }
        public void OnDestroy() // remove an enemy when it's destroyed
        {
            _enemies.Remove(this);
        }
        // Combat
        public void Attack(Player player)
        {
            player.GameObject.GetComponent<HealthSystem>()?.TakeDamage(10);
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
    }
}
