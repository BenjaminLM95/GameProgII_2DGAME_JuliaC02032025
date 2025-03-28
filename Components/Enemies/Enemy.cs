using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGame_Julia_C02032025.Components.Enemies
{
    public enum EnemyType
    {
        Slime,      // Basic enemy
        Ghost       // Elite enemy
    }
    public class EnemyConfig
    {
        public string SpriteName { get; set; }
        public int MaxHealth { get; set; }
        public int Damage { get; set; }
        public float MovementSpeed { get; set; }

        // Default configurations for enemy types
        public static EnemyConfig GetConfig(EnemyType type)
        {
            return type switch
            {
                EnemyType.Slime => new EnemyConfig
                {
                    SpriteName = "enemy",
                    MaxHealth = 30,
                    Damage = 5,
                    MovementSpeed = 1f
                },
                EnemyType.Ghost => new EnemyConfig
                {
                    SpriteName = "ghost",
                    MaxHealth = 50,
                    Damage = 10,
                    MovementSpeed = 1.5f
                },
                _ => throw new ArgumentException("Unknown enemy type")
            };
        }
    }

    internal class Enemy : Component
    {
        // Configurable enemy properties
        public EnemyType Type { get; private set; }
        private EnemyConfig config;
        // Component references
        private Globals globals;
        private HealthSystem healthSystem;
        private Pathfinding pathfinding;
        private TileMap tileMap;
        private Sprite enemySprite;

        // ---------- VARIABLES ---------- //
        private int minEnemyCount = 2;
        private int maxEnemyCount = 10;
        // Static list of all enemies
        private static List<Enemy> _enemies = new List<Enemy>();

        // State variables
        private bool isStunned = false;
        public bool enemyMovedOntoPlayerTile { get; private set; }

        // Constructor with enemy type
        public Enemy(EnemyType type = EnemyType.Slime)
        {
            Type = type;
            config = EnemyConfig.GetConfig(type);
            globals = Globals.Instance;
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

            // Setup sprite
            enemySprite = GameObject.GetComponent<Sprite>();
            if (enemySprite == null)
            {
                Debug.WriteLine("Enemy: Sprite component is NULL!");
            }
            else
            {
                // Load specific sprite based on enemy type
                enemySprite.LoadSprite(config.SpriteName);
            }

            healthSystem = GameObject.GetComponent<HealthSystem>() ?? GameObject.FindObjectOfType<HealthSystem>();
            GameObject.AddComponent(new HealthSystem(
                    maxHealth: config.MaxHealth,
                    type: HealthSystem.EntityType.Enemy
                ));

            tileMap = globals._mapSystem?.Tilemap;
            if (tileMap == null)
            {
                tileMap = GameObject.FindObjectOfType<TileMap>();
            }
            if (tileMap == null)
            {
                Debug.WriteLine("Enemy: CRITICAL - Could not find TileMap!");
            }
            pathfinding = GameObject.GetComponent<Pathfinding>();
            pathfinding = GameObject.FindObjectOfType<Pathfinding>();
            if (pathfinding != null && tileMap != null)
            {
                Debug.WriteLine("Enemy: Attempting to reinitialize Pathfinding");
                try
                {
                    pathfinding.InitializePathfinding(tileMap);
                }
                catch (Exception ex) {
                    Debug.WriteLine($"Enemy: Pathfinding initialization failed - {ex.Message}");
                }
            }
            else {
                Debug.WriteLine($"Enemy: Pathfinding initialization failed. Pathfinding: {(pathfinding == null ? "NULL" : "Found")}, TileMap: {(tileMap == null ? "NULL" : "Found")}");
            }
        }
        public void Update()
        {
            if (isStunned) return; // Skip turn if stunned

            Player player = GameObject.FindObjectOfType<Player>();
            if (player == null) return;
            Debug.WriteLine($"Enemy at {GameObject.Position} processing turn");

            if (IsNextToPlayer(player))
            {
                Debug.WriteLine("Enemy: Attacking player");
                Attack(player);
            }
            else
            {
                Debug.WriteLine("Enemy: moving towards player");
                MoveTowardsPlayer(player);
            }
        }

        public bool IsNextToPlayer(Player player)
        {
            Debug.WriteLine("Enemy: checking if enemy is next to player");
            Vector2 playerPos = player.GameObject.Position;
            Vector2 enemyPos = GameObject.Position;
            return Math.Abs(playerPos.X - enemyPos.X) <= 32 && Math.Abs(playerPos.Y - enemyPos.Y) <= 32;
        }

        // Tilemap Movement
        public void MoveTowardsPlayer(Player player)
        {
            pathfinding = GameObject.GetComponent<Pathfinding>();
            if (pathfinding == null)
            {
                Debug.WriteLine("Enemy: Pathfinding component is NULL");
                return;
            }

            // Ensure nodeMap is initialized
            if (pathfinding.nodeMap == null)
            {
                Debug.WriteLine("Enemy: Pathfinding nodeMap is NULL");
                return;
            }

            Vector2 playerPos = player.GameObject.Position / 32;
            Vector2 enemyPos = GameObject.Position / 32;

            Point enemyPoint = new Point((int)enemyPos.X, (int)enemyPos.Y);
            Point playerPoint = new Point((int)playerPos.X, (int)playerPos.Y);

            Debug.WriteLine($"Enemy: position: {enemyPoint}, Player position: {playerPoint}");

            List<Point> path = pathfinding.FindPath(enemyPoint, playerPoint);

            // Correct null and empty path checking
            if (path == null)
            {
                Debug.WriteLine($"Enemy: Path finding completely failed.");
                return;
            }
            if (path.Count <= 1)
            {
                Debug.WriteLine($"Enemy: Path is too short. Path count: {path.Count}");
                return;
            }

            // Move to the next tile in the path (path[1] is the next step)
            Point nextTile = path[1];
            // Check if the next tile is walkable and not occupied
            if (IsTileWalkable(nextTile))
            {
                Vector2 newPosition = new Vector2(nextTile.X * 32, nextTile.Y * 32);
                GameObject.Position = newPosition;
                Debug.WriteLine($"Enemy moved to {newPosition}");
            }
            else
            {
                Debug.WriteLine($"Enemy: Next tile {nextTile} is not walkable!");
            }
        }
        private bool IsTileWalkable(Point tile)
        {
            // Check if tile is a floor tile
            if (tileMap == null || tileMap.GetTileAt(tile.X, tile.Y) == null ||
                tileMap.GetTileAt(tile.X, tile.Y).Texture != tileMap.floorTexture)
            {
                Debug.WriteLine($"Tile {tile} is not a valid floor tile");
                return false;
            }

            // Check if player is on this tile
            Player player = GameObject.FindObjectOfType<Player>();
            if (player != null && player.GameObject != null)
            {
                Vector2 playerTile = player.GameObject.Position / 32;
                if (new Point((int)playerTile.X, (int)playerTile.Y) == tile)
                {
                    Debug.WriteLine($"Tile {tile} is occupied by player");
                    return false;
                }
            }

            // Check if any other enemy is on this tile
            List<Enemy> enemies = GetEnemies();
            foreach (Enemy enemy in enemies)
            {
                if (enemy != this && enemy.GameObject != null) // Exclude the current enemy and check for null
                {
                    Vector2 enemyTile = enemy.GameObject.Position / 32;
                    if (new Point((int)enemyTile.X, (int)enemyTile.Y) == tile)
                    {
                        Debug.WriteLine($"Tile {tile} is occupied by another enemy");
                        return false;
                    }
                }
            }

            return true;
        }
        public List<Enemy> GetEnemies() 
        {
            // Remove any enemies with null GameObjects
            _enemies.RemoveAll(e => e == null || e.GameObject == null);
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
