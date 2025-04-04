using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGame_Julia_C02032025.Components.Enemies
{
    // ========== ENUM ========== //
    public enum EnemyType
    {
        Slime,      // Basic enemy
        Ghost,      // Elite enemy
        Archer      // Ranged enemy
    }
    // ========== HELPER CLASS ========== //
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
                EnemyType.Archer => new EnemyConfig
                {
                    SpriteName = "archer",
                    MaxHealth = 50,
                    Damage = 10,
                    MovementSpeed = 0f
                },
                _ => throw new ArgumentException("Unknown enemy type")
            };
        }
    }

    // ========== COMPONENT CLASS ========== //
    internal class Enemy : Component
    {
        // Configurable enemy properties
        public EnemyType Type { get; private set; }
        public EnemyConfig config { get; private set; }

        // ---------- REFERENCES ---------- //
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
        public static List<GameObject> AllEnemies = new(); // tracks gameobjects
        public static List<Enemy> _enemies = new List<Enemy>(); // tracks this component

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
            if (globals == null) {
                Debug.WriteLine("Enemy: globals is NULL!");
                throw new InvalidOperationException("Globals instance could not be initialized");
            }

            // Setup sprite
            enemySprite = GameObject.GetComponent<Sprite>();
            if (enemySprite == null) {
                Debug.WriteLine("Enemy: sprite is NULL!");
            }
            else
            {
                enemySprite.LoadSprite(config.SpriteName);
            }

            healthSystem = GameObject.GetComponent<HealthSystem>() ?? GameObject.FindObjectOfType<HealthSystem>();
            
            tileMap = globals._mapSystem?.Tilemap;
            if (tileMap == null) {
                tileMap = GameObject.FindObjectOfType<TileMap>();
            }
            if (tileMap == null) {
                Debug.WriteLine("Enemy: CRITICAL - Could not find TileMap!");
            }

            pathfinding = GameObject.GetComponent<Pathfinding>();
            pathfinding = GameObject.FindObjectOfType<Pathfinding>();
            if (pathfinding != null && tileMap != null)
            {
                Debug.WriteLine("Enemy: Attempting to reinitialize Pathfinding");
                try {
                    pathfinding.InitializePathfinding(tileMap); // PATFINDING: initialize
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
        }

        public bool IsNextToPlayer(Player player, bool debug = false)
        {
            if (debug) Debug.WriteLine("Enemy: checking if enemy is next to player");
            Vector2 playerPos = player.GameObject.Position;
            Vector2 enemyPos = GameObject.Position;
            return Math.Abs(playerPos.X - enemyPos.X) <= 32 && Math.Abs(playerPos.Y - enemyPos.Y) <= 32;
        }

        // Tilemap Movement
        public void MoveTowardsPlayer(Player player, bool debug = false)
        {
            pathfinding = GameObject.GetComponent<Pathfinding>();
            if (pathfinding == null) {
                if (debug) Debug.WriteLine("Enemy: Pathfinding component is NULL");
                return;
            }

            // Ensure nodeMap is initialized
            if (pathfinding.nodeMap == null) {
                if (debug) Debug.WriteLine("Enemy: Pathfinding nodeMap is NULL");
                return;
            }

            Vector2 playerPos = player.GameObject.Position / 32;
            Vector2 enemyPos = GameObject.Position / 32;

            Point enemyPoint = new Point((int)enemyPos.X, (int)enemyPos.Y);
            Point playerPoint = new Point((int)playerPos.X, (int)playerPos.Y);

            if (debug) Debug.WriteLine($"Enemy: position: {enemyPoint}, Player position: {playerPoint}");

            List<Point> path = pathfinding.FindPath(enemyPoint, playerPoint);

            // Correct null and empty path checking
            if (path == null) {
                if (debug) Debug.WriteLine($"Enemy: Path finding completely failed.");
                return;
            }
            if (path.Count <= 1) {
                if (debug) Debug.WriteLine($"Enemy: Path is too short. Path count: {path.Count}");
                return;
            }

            // Move to the next tile in the path (path[1] is the next step)
            Point nextTile = path[1];
            // Check if the next tile is walkable and not occupied
            if (IsTileWalkable(nextTile))
            {
                Vector2 newPosition = new Vector2(nextTile.X * 32, nextTile.Y * 32);
                GameObject.Position = newPosition;
                if (debug) Debug.WriteLine($"Enemy moved to {newPosition}");
            }
            else {
                if (debug) Debug.WriteLine($"Enemy: Next tile {nextTile} is not walkable!");
            }
        }

        // enemies can only walk on floor tiles, not on the player or other enemies
        public bool IsTileWalkable(Point tile, bool debug = false)
        {
            // Check if tile is a floor tile
            if (tileMap == null || tileMap.GetTileAt(tile.X, tile.Y) == null ||
                tileMap.GetTileAt(tile.X, tile.Y).Texture != tileMap.floorTexture)
            {
                if (debug) Debug.WriteLine($"Tile {tile} is not a valid floor tile");
                return false;
            }

            // Check if player is on this tile
            Player player = GameObject.FindObjectOfType<Player>();
            if (player != null && player.GameObject != null)
            {
                Vector2 playerTile = player.GameObject.Position / 32;
                if (new Point((int)playerTile.X, (int)playerTile.Y) == tile)
                {
                    if (debug) Debug.WriteLine($"Tile {tile} is occupied by player");
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
                        if (debug) Debug.WriteLine($"Tile {tile} is occupied by another enemy");
                        return false;
                    }
                }
            }
            return true;
        }
        public List<Enemy> GetEnemies() // returns a list of all active enemies in the scene
        {
            // Remove any enemies with null GameObjects
            _enemies.RemoveAll(e => e == null || e.GameObject == null);
            return _enemies;
        }
        public override void OnDestroy() // remove an enemy when it's destroyed,
                                // but they still exist rn according to turn indicator
        { // method is ref component.OnDestroy() right now
            AllEnemies.Remove(GameObject);
            // need to destroy gameobject as well
        }
        // Combat
        public void Attack(Player player)
        {
            player.GameObject.GetComponent<HealthSystem>()?.ModifyHealth(-10);
        }
    }

    public static class EnemySpawner
    {
        public static void RespawnEnemies(int level)
        {
            // Clean up old enemies
            foreach (GameObject enemy in Enemy.AllEnemies)
            {
                Globals.Instance._scene.RemoveGameObject(enemy);
            }
            Enemy.AllEnemies.Clear();
            Enemy._enemies.Clear();

            // Clamp enemy count and spawn
            int enemyCount = Math.Clamp(level, 2, 10);

            for (int i = 0; i < enemyCount; i++)
            {
                GameObject enemyObject = new GameObject();

                // Create components
                Enemy newEnemy = new Enemy(); // defaults to Slime
                Sprite enemySprite = new Sprite();
                Pathfinding enemyPathfinding = new Pathfinding();
                HealthSystem enemyHealth = new HealthSystem(
                    maxHealth: newEnemy.config.MaxHealth,
                    type: HealthSystem.EntityType.Enemy
                );

                // Add components
                enemyObject.AddComponent(newEnemy);
                enemyObject.AddComponent(enemySprite);
                enemyObject.AddComponent(enemyPathfinding);
                enemyObject.AddComponent(enemyHealth);

                // Load sprite
                enemySprite.LoadSprite(newEnemy.config.SpriteName);

                // Get a valid tile
                Vector2 randomTile = Globals.Instance._mapSystem.GetRandomEmptyTile();
                if (randomTile == new Vector2(-1, -1))
                {
                    Debug.WriteLine("EnemySpawner: No valid spawn tile found!");
                    continue;
                }

                enemyObject.Position = randomTile;

                // Initialize pathfinding
                TileMap tileMap = Globals.Instance._mapSystem.Tilemap;
                if (tileMap != null)
                {
                    enemyPathfinding.InitializePathfinding(tileMap);
                    Debug.WriteLine($"EnemySpawner: Spawned enemy at {randomTile}");
                }
                else
                {
                    Debug.WriteLine("EnemySpawner: Cannot initialize pathfinding - TileMap is NULL");
                }

                // Track and add to scene
                Enemy.AllEnemies.Add(enemyObject);
                Globals.Instance._scene.AddGameObject(enemyObject);
            }
        }
    }
}
// enemies not respawning in each floor, once they are all killed there are none on the next floor
