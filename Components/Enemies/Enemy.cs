using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGame_Julia_C02032025.Components.Enemies
{
    // ========== ENUM ========== //
    public enum EnemyType
    {
        Slime,      // Basic enemy
        Ghost,      // Elite enemy
        Archer,     // Ranged enemy
        Boss        // Boss enemy
    }
    // ========== HELPER CLASS ========== //
    public class EnemyConfig
    {
        public string SpriteName { get; set; }
        public int MaxHealth { get; set; }
        public int Damage { get; set; }
        public float MovementSpeed { get; set; }

        public int moneyReward { get; set; }



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
                    MovementSpeed = 1f,
                    moneyReward = 15

                },
                EnemyType.Ghost => new EnemyConfig
                {
                    SpriteName = "ghost",
                    MaxHealth = 20,
                    Damage = 8,
                    MovementSpeed = 1.5f,
                    moneyReward = 10

                },
                EnemyType.Archer => new EnemyConfig
                {
                    SpriteName = "archer",
                    MaxHealth = 40,
                    Damage = 10,
                    MovementSpeed = 1f,
                    moneyReward = 20

                },
                EnemyType.Boss => new EnemyConfig
                {
                    SpriteName = "boss",
                    MaxHealth = 100,
                    Damage = 20,
                    MovementSpeed = 1f,
                    moneyReward = 50


                },
                _ => throw new ArgumentException("Unknown enemy type")
            }; 
        }
    }

    // ========== COMPONENT CLASS ========== //
    internal class Enemy : Component, ITurnTaker
    {
        // Configurable enemy properties
        public EnemyType Type { get; private set; }
        public EnemyConfig config { get; private set; }

        // ---------- REFERENCES ---------- //
        protected Globals globals;
        public HealthSystem healthSystem;
        protected TileMap tileMap;
        protected Sprite enemySprite;
        protected Player player;

        // ---------- VARIABLES ---------- //
        protected bool hasMoved = false;
        protected bool isStunned = false;
        public bool enemyMovedOntoPlayerTile { get; protected set; }
        public bool hasTakenTurn { get; set; } = false;
       

        public static List<GameObject> AllEnemies = new(); // tracks gameobjects
        public static List<Enemy> _enemies = new List<Enemy>(); // tracks this component

        // Constructor with enemy type
        protected Enemy(EnemyType type)
        {
            Type = type;
            config = EnemyConfig.GetConfig(type);
            globals = Globals.Instance;
            _enemies.Add(this);
        }

        // ---------- METHODS ---------- //
        public override void OnAddedToGameObject() 
        {
            InitializeEnemy(); 
        }
        
        public override void Start()
        {
            globals = globals ?? Globals.Instance; // globals
            if (globals == null) {
                Debug.WriteLine("Enemy: globals is NULL!");
                throw new InvalidOperationException("Globals instance could not be initialized");
            }

            // set up sprite
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

            player = GameObject.FindObjectOfType<Player>();
        }
        public virtual void StartTurn(TurnManager manager)
        {
            if (IsDead())
            {
                manager.EndTurn();
                return;
            }

            hasMoved = false;
            hasTakenTurn = false;

            if (IsNextToPlayer(player))
            {
                Debug.WriteLine("Enemy: Enemy is next to player, attacking!");
                Attack(player);
            }
            else
            {
                Debug.WriteLine("Enemy: Enemy is not next to player, moving...");
                MoveTowardsPlayer(player);
            }

            manager.EndTurn();
        }
        public bool IsDead()
        {
            return healthSystem?.CurrentHealth <= 0;
        }

        public virtual void Update(float deltaTime)
        {
            if (player == null)
                player = GameObject.FindObjectOfType<Player>();
            if (player == null) return;
            Debug.WriteLine($"Enemy at {GameObject.Position} processing turn");
        }

        public EnemyType getEnemyType() 
        {
            return Type; 
        }

        public bool IsNextToPlayer(Player player, bool debug = false)
        {
            if (player == null || player.GameObject == null)
            {
                if (debug) Debug.WriteLine("Enemy: Cannot check proximity - player is null");
                return false;
            }

            Vector2 playerPos = player.GameObject.Position;
            Vector2 enemyPos = GameObject.Position;

            // calculate tile-based distance
            int tileDistanceX = (int)Math.Abs((playerPos.X - enemyPos.X) / 32);
            int tileDistanceY = (int)Math.Abs((playerPos.Y - enemyPos.Y) / 32);

            // adjacent if exactly one tile away (horizontally or vertically)
            bool isAdjacent = (tileDistanceX == 1 && tileDistanceY == 0) || (tileDistanceX == 0 && tileDistanceY == 1);

            if (debug) Debug.WriteLine($"Enemy: IsNextToPlayer = {isAdjacent}, distances X={tileDistanceX}, Y={tileDistanceY}");
            return isAdjacent;
        }
        public void SetPlayer(Player playerRef)
        {
            player = playerRef;
        }

        // Tilemap Movement
        public virtual void MoveTowardsPlayer(Player player, bool debug = false)
        {
            // basic movement logic - move one tile towards the player
            Vector2 playerPos = player.GameObject.Position;
            Vector2 enemyPos = GameObject.Position;

            // calculate direction to player
            Vector2 direction = playerPos - enemyPos;

            // normalize to get the primary direction
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                // move horizontally
                direction.Y = 0;
                direction.X = direction.X > 0 ? 32 : -32;
            }
            else
            {
                // move vertically
                direction.X = 0;
                direction.Y = direction.Y > 0 ? 32 : -32;
            }

            // calculate target position
            Vector2 targetPos = enemyPos + direction;
            Point targetTile = new Point((int)targetPos.X / 32, (int)targetPos.Y / 32);

            // check if tile is walkable
            if (IsTileWalkable(targetTile))
            {
                if (debug) Debug.WriteLine($"Enemy: Moving from {enemyPos} to {targetPos}");
                GameObject.Position = targetPos;
                hasMoved = true;
            }
            else
            {
                if (debug) Debug.WriteLine($"Enemy: Cannot move to tile {targetTile}");
            }
        }

        // Enemies can only walk on floor tiles, not on the player or other enemies
        public bool IsTileWalkable(Point tile, bool debug = false)
        {
            // check if tile is a floor tile
            if (tileMap == null || tileMap.GetTileAt(tile.X, tile.Y) == null ||
                tileMap.GetTileAt(tile.X, tile.Y).Texture != tileMap.floorTexture)
            {
                if (debug) Debug.WriteLine($"Tile {tile} is not a valid floor tile");
                return false;
            }

            // check if player is on this tile
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

            // check if any other enemy is on this tile
            List<Enemy> enemies = GetEnemies();
            foreach (Enemy enemy in enemies)
            {
                if (enemy != this && enemy.GameObject != null) // exclude the current enemy and check for null
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
        public bool IsTileSeeThrough(Point tile) // for enemy projectile to recognize travel-able tiles
        {
            var t = tileMap?.GetTileAt(tile.X, tile.Y);

            if (t == null)
            {
                Debug.WriteLine($"Tile {tile} is null");
                return false;
            }

            // if the tile is a wall or obstacle, block vision
            if (t.Texture == tileMap.obstacleTexture || t.Texture == tileMap.wallTexture)
            {
                Debug.WriteLine($"Tile {tile} is blocking LOS (wall or obstacle)");
                return false;
            }
            return true;
        }

        public virtual void InitializeEnemy()
        {
            AllEnemies.Add(GameObject);
            _enemies.Add(this);

            Debug.WriteLine($"Enemy: Initialized at {this.GameObject.Position}");
        }
        
        public List<Enemy> GetEnemies() // Returns a list of all active enemies in the scene
        {
            // remove any enemies with null GameObjects
            _enemies.RemoveAll(e => e == null || e.GameObject == null);
            return _enemies;
        }
        public override void OnDestroy() // remove an enemy when it's destroyed
        { 
            Debug.WriteLine("Enemy: OnDestroy called");

            player.currency += config.moneyReward;  // The player gets the money reward by killing the enemy
            player.numKills++; // The player kill count increases by one

            if(Type.ToString() == "Boss") 
            {
                player.killBoss = true; 
            }

            AllEnemies.Remove(GameObject);
            _enemies.Remove(this);

            // remove from TurnManager
            TurnManager.Instance?.RemoveTurnTaker(this); // not working?
        }
        // Combat
        public virtual void Attack(Player player)
        {
            player.GameObject.GetComponent<HealthSystem>()?.ModifyHealth(-10);
        }
    }
    // ========== SPAWNING CLASS ========== //
    public static class EnemySpawner
    {
        public static void RespawnEnemies(int level)
        {
            // Clean up old enemies
            foreach (GameObject enemy in Enemy.AllEnemies.ToList())
            {
                enemy.Destroy();
            }
            Enemy.AllEnemies.Clear();
            Enemy._enemies.Clear();

            // Clamp enemy count and spawn
            int enemyCount = Math.Clamp(level, 2, 10);

            // Mix of enemy types based on level
            int slimeCount = Math.Max(2, enemyCount / 2);
            int ghostCount = Math.Max(1, enemyCount / 2);
            int archerCount = Math.Max(2, enemyCount - slimeCount - ghostCount);

            //spawn boss
            SpawnEnemy(new BossEnemy());

            // Spawn slimes
            for (int i = 0; i < slimeCount; i++)
            {
                SpawnEnemy(new BasicEnemy());
            }

            // Spawn ghosts 
            for (int i = 0; i < ghostCount; i++)
            {
                SpawnEnemy(new GhostEnemy());
            }

            // Spawn archers
            for (int i = 0; i < archerCount; i++)
            {
                SpawnEnemy(new RangedEnemy());
            }
        }

        public static void SpawnBoss(bool debug = false)
        {
            Debug.WriteLine("EnemySpawner: Explicitly spawning BOSS");
            // Clean up old enemies
            foreach (GameObject enemy in Enemy.AllEnemies.ToList())
            {
                enemy.Destroy();
            }
            Enemy.AllEnemies.Clear();
            Enemy._enemies.Clear();

            SpawnEnemy(new BossEnemy());
        }

        private static void SpawnEnemy(Enemy enemyComponent, bool debug = false)
        {
            GameObject enemyObject = new GameObject();

            // create components
            Sprite enemySprite = new Sprite();
            HealthSystem enemyHealth = new HealthSystem(
                maxHealth: enemyComponent.config.MaxHealth,
                type: HealthSystem.EntityType.Enemy
            );

            // add components
            enemyObject.AddComponent(enemyComponent);
            enemyObject.AddComponent(enemySprite);
            enemyObject.AddComponent(enemyHealth);

            // Register enemy in global lists - !!! issue !!! this is making enemies stay after death
            //Enemy.AllEnemies.Add(enemyObject);
            //Enemy._enemies.Add(enemyComponent);

            // load sprite
            enemySprite.LoadSprite(enemyComponent.config.SpriteName);
            if (enemySprite.Texture == null)
            {
                Debug.WriteLine($"EnemySpawner: FAILED to load sprite for {enemyComponent.Type}!");
            }
            else
            {
                Debug.WriteLine($"EnemySpawner: Successfully loaded sprite for {enemyComponent.Type}!");
            }

            // get a valid tile
            Vector2 randomTile = Globals.Instance._mapSystem.GetRandomEmptyTile();
            if (randomTile == new Vector2(-1, -1))
            {
                if (debug) Debug.WriteLine("EnemySpawner: No valid spawn tile found!");
                return;
            }

            enemyObject.Position = randomTile;
            Debug.WriteLine($"EnemySpawner: {enemyComponent.Type} spawned at {randomTile}");

            // track and add to scene
            Globals.Instance._scene.AddGameObject(enemyObject);

            // Register with TurnManager
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.RegisterTurnTaker(enemyComponent);
                if (debug) Debug.WriteLine($"EnemySpawner: Added {enemyComponent.Type} to TurnManager");
            }
        }
    }
}
