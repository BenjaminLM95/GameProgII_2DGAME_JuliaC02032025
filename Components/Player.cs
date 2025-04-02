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
    /// <summary>
    /// Handles player input, movement, and interactions with the map (e.g., obstacles, exit).
    /// </summary>
    internal class Player : Component
    {
        Globals globals;
        private TileMap tileMap;
        private HealthSystem healthSystem;
        private Sprite playerSprite;
        private Inventory inventory;

        // ---------- VARIABLES ---------- //

        private float speed = 300f;
        private int tileSize = 32;
        private int spriteScale = 1;

        // Turn based combat
        public bool hasMovedThisTurn = false;
        public bool playerMovedOntoEnemyTile { get; private set; }

        public Vector2 LastMovementDirection { get; private set; } = Vector2.UnitX; // Default facing right

        public Player() { }
        public Player(TileMap tileMap)
        {
            this.tileMap = tileMap;
        }

        // ---------- METHODS ---------- //

        // Initializes the player by finding the map system and tile map.
        public override void Start()
        {
            // null checks & component assignment
            globals = Globals.Instance; // globals
            
            playerSprite = GameObject.GetComponent<Sprite>(); // sprite
            if (playerSprite == null)
            {
                //Debug.WriteLine("Player: Sprite component is NULL!");
            }

            healthSystem = GameObject.GetComponent<HealthSystem>(); // HealthSystem
            if (healthSystem == null) {
                Debug.WriteLine("Player: HealthSystem component is NULL!");
            }
            else {
                globals._healthSystem = healthSystem;
            }

            globals._mapSystem = GameObject.FindObjectOfType<MapSystem>();  // mapsystem
            tileMap = globals._mapSystem?.Tilemap; // TileMap

            inventory = GameObject.GetComponent<Inventory>(); // inventory
            if (inventory == null)
            {
                Inventory newInventory = new Inventory();
                GameObject.AddComponent(newInventory);
                inventory = newInventory;
                Debug.WriteLine("Player: Inventory component was missing, added dynamically.");
            }

            Debug.WriteLine("Player: Waiting for map initialization and start position...");
            MoveToStartTile();
        }

        // Updates the player's position based on input, checking for obstacles before moving.
        public override void Update(float deltaTime)
        {
            // need to keep checking for button pressed

            if (hasMovedThisTurn || !Combat.Instance.isPlayerTurn) return;

            if (tileMap == null)  
            {
                tileMap = globals._mapSystem?.Tilemap;
                if (tileMap == null) return;
            }
            if (healthSystem == null)
            {
                healthSystem = GameObject.GetComponent<HealthSystem>();
                Debug.WriteLine("Player: Still trying to find HealthSystem component...");
            }
            ReadInput(); // WASD and 1,2,3,4,5, check tiles = Obstacle/Enemy/Item
        }
        private void ReadInput(bool debug = false)
        {
            // Input
            Vector2 currentPos = GameObject.Position;
            Vector2 targetPos = currentPos;
            bool moved = false;
            Vector2 movementDirection = Vector2.Zero;

            KeyboardState KeyboardState = Keyboard.GetState();

            if (KeyboardState.IsKeyDown(Keys.W))
            {
                if(debug) Debug.WriteLine($"Player: moving UP");
                targetPos.Y -= tileSize;
                moved = true;
                movementDirection = -Vector2.UnitY;
            }
            if (KeyboardState.IsKeyDown(Keys.A))
            {
                if (debug) Debug.WriteLine($"Player: moving LEFT");
                targetPos.X -= tileSize;
                moved = true;
                movementDirection = -Vector2.UnitX;
            }
            if (KeyboardState.IsKeyDown(Keys.S))
            {
                if (debug) Debug.WriteLine($"Player: moving DOWN");
                targetPos.Y += tileSize;
                moved = true;
                movementDirection = Vector2.UnitY;
            }
            if (KeyboardState.IsKeyDown(Keys.D))
            {
                if (debug) Debug.WriteLine($"Player: moving RIGHT");
                targetPos.X += tileSize;
                moved = true;
                movementDirection = Vector2.UnitX;
            }
            // Check if a number key (1-5) is pressed to use an item
            for (int i = 0; i < 5; i++)
            {
                Keys key = (Keys)((int)Keys.D1 + i); // Maps 1-5 keys
                if (KeyboardState.IsKeyDown(key))
                {
                    if (debug) Debug.WriteLine($"Player: reading keys 1-5"); // works
                    inventory.UseInventoryItem(i);
                }
            }

            if (moved && !hasMovedThisTurn)
            {
                // Update last movement direction
                if (movementDirection != Vector2.Zero)
                {
                    LastMovementDirection = movementDirection;
                }
                // Convert target position to tile coordinates
                Point targetTilePos = GetTileCoordinates(targetPos);

                // Check if target tile is an obstacle before moving
                if (!IsObstacle(targetTilePos))
                {
                    GameObject.Position = targetPos;
                    if (debug) Debug.WriteLine($"Player: moved to position - {targetPos}");
                    hasMovedThisTurn = true;
                    // Combat
                    // If the player successfully moves, advance the turn
                    Combat.Instance.AdvanceToNextTurn();
                }
                if (IsExit(targetTilePos)) // if tile is exit, Load Next Level
                {
                    GameObject.Position = targetPos;
                    hasMovedThisTurn = true;
                    // gen rand next level
                    globals._mapSystem.LoadNextLevel();
                    // Advance turn after moving to exit
                    Combat.Instance.AdvanceToNextTurn();
                }
                if (IsItem(targetTilePos)) // if tile is an Item, add it to inventory
                {
                    GameObject.Position = targetPos;
                    if (debug) Debug.WriteLine($"Player: moved to ITEM position - {targetPos}");
                    hasMovedThisTurn = true;

                    // Pick up the item at this position
                    PickUpItem(targetTilePos);

                    Combat.Instance.AdvanceToNextTurn();
                    if (debug) Debug.WriteLine("Current Inventory: "
                        + string.Join(", ", inventory.items.ConvertAll(i => i.Type.ToString())));
                }
            }
        }

        // Convert world position to tile coordinates
        private Point GetTileCoordinates(Vector2 worldPosition)
        {
            return new Point(
                (int)(worldPosition.X / (tileSize * spriteScale)),
                (int)(worldPosition.Y / (tileSize * spriteScale)));
        }
        
        public Vector2 GetPlayerFacingDirection() // get player's facing direction
        {
            return LastMovementDirection;
        }

        // Check if the target tile contains an "obstacle" or "wall" tile
        private bool IsObstacle(Point tileCoordinates)
        {
            if (tileMap == null) return false;

            Sprite targetTile = tileMap.GetTileAt(tileCoordinates.X, tileCoordinates.Y);
            // check for floor & wall obstacles
            if (targetTile != null && targetTile.Texture == tileMap.obstacleTexture || targetTile.Texture == tileMap.wallTexture)
            {
                Debug.WriteLine($"Player: Obstacle at {tileCoordinates.X}, {tileCoordinates.Y}!");
                return true;
            }

            // Check for enemy tiles
            Enemy firstEnemy = GameObject.FindObjectOfType<Enemy>();
            if (firstEnemy != null) // keeps from crashing when there are no enemies to check
            {
                List<Enemy> enemies = GameObject.FindObjectOfType<Enemy>().GetEnemies();
                foreach (Enemy enemy in enemies)
                {
                    if (enemy.GameObject != null)
                    {
                        Vector2 enemyTile = enemy.GameObject.Position / 32;
                        Point enemyTilePoint = new Point((int)enemyTile.X, (int)enemyTile.Y);

                        if (enemyTilePoint == tileCoordinates)
                        {
                            Debug.WriteLine($"Player: Enemy tile at {tileCoordinates.X}, {tileCoordinates.Y}!");
                            // integrate combat
                            AttackEnemyAtPosition(enemyTilePoint);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool IsItem(Point tileCoordinates, bool debug = false)
        {
            // Get the Items component from globals
            Items itemsComponent = Globals.Instance._items;
            if (itemsComponent == null)
            {
                if(debug) Debug.WriteLine("Player: Cannot check for items, Items component is null");
                return false;
            }

            List<Item> items = itemsComponent.GetItems();
            if (debug) Debug.WriteLine($"Player: Checking for items at {tileCoordinates.X}, {tileCoordinates.Y}. Found {items.Count} items in world.");

            foreach (Item item in items)
            {
                if (item != null)
                {
                    // Convert item world position to tile coordinates
                    int itemTileX = (int)(item.Position.X / tileSize);
                    int itemTileY = (int)(item.Position.Y / tileSize);
                    Point itemTilePoint = new Point(itemTileX, itemTileY);

                    if (debug) Debug.WriteLine($"Player: Item {item.Type} is at tile {itemTilePoint.X}, {itemTilePoint.Y}");

                    if (itemTilePoint.Equals(tileCoordinates))
                    {
                        if (debug) Debug.WriteLine($"Player: Found item {item.Type} at player's target position!");
                        return true;
                    }
                }
            }
            return false;
        }

        // Checks if the player's current position is on an "exit" tile.
        public bool IsExit(Point playerPosition)
        {
            if (tileMap == null) return false;

            Sprite targetTile = tileMap.GetTileAt(playerPosition.X, playerPosition.Y);
            if (targetTile != null && targetTile.Texture == tileMap.exitTexture)
            {
                Debug.WriteLine($"Player: Exit found at {playerPosition.X}, {playerPosition.Y}!");
                return true;
            }
            return false;
        }

        // Finds the "start" tile and moves the player to it
        public void MoveToStartTile()
        {
            if (tileMap == null) return;

            for (int y = 0; y < tileMap.mapHeight; y++)
            {
                for (int x = 0; x < tileMap.mapWidth; x++)
                {
                    Sprite tile = tileMap.GetTileAt(x, y);
                    if (tile != null && tile.Texture == tileMap.startTexture)
                    {
                        GameObject.Position = new Vector2(x * tileSize, y * tileSize);
                        Debug.WriteLine($"Player: Spawned at start tile ({x}, {y}).");
                        return;
                    }
                }
            }
            Debug.WriteLine("Player: Start tile not found!");
        }
      
        public void ResetTurn()
        {
            // Reset movement state
            hasMovedThisTurn = false;
            playerMovedOntoEnemyTile = false;

            Debug.WriteLine("Player: Turn reset - ready to move again");
            Debug.WriteLine($"Player: current health: {healthSystem.CurrentHealth}");
        }
        public void TakeDamage(int damage) => healthSystem.ModifyHealth(-damage);

        // Combat
        private void AttackEnemyAtPosition(Point enemyTilePosition)
        {
            // Find the enemy at the given tile position
            List<Enemy> enemies = GameObject.FindObjectOfType<Enemy>().GetEnemies();
            foreach (Enemy enemy in enemies)
            {
                Vector2 enemyTile = enemy.GameObject.Position / 32;
                Point enemyTilePoint = new Point((int)enemyTile.X, (int)enemyTile.Y);

                if (enemyTilePoint == enemyTilePosition)
                {
                    // Attack the enemy if found at the target position
                    Attack(enemy, 10);
                    break;
                }
            }
        }
        public void Attack(Enemy enemy, int amountdmg)
        {
            Debug.WriteLine("Player: Attacked enemy for 10 dmg");
            if (enemy != null) {
                enemy.GameObject.GetComponent<HealthSystem>()?.ModifyHealth(-amountdmg);
            }
        }

        private void PickUpItem(Point targetTilePos, bool debug = false) // items/inventory
        {
            // Get the Items component from globals
            Items itemsComponent = Globals.Instance._items;
            if (itemsComponent == null)
            {
                if(debug) Debug.WriteLine("Player: Cannot pick up item, Items component is null");
                return;
            }

            List<Item> items = itemsComponent.GetItems();
            foreach (Item item in items)
            {
                if (item != null)
                {
                    // Convert item world position to tile coordinates
                    int itemTileX = (int)(item.Position.X / tileSize);
                    int itemTileY = (int)(item.Position.Y / tileSize);
                    Point itemTilePoint = new Point(itemTileX, itemTileY);

                    if (itemTilePoint.Equals(targetTilePos))
                    {
                        // Add item to inventory
                        if (inventory != null)
                        {
                            // need to check if inv is full before removing item
                            int previousCount = inventory.items.Count;
                            inventory.AddItem(item);

                            // Only remove the item if it was actually added to player's inventory
                            if (inventory.items.Count > previousCount)
                            {
                                itemsComponent.GetItems().Remove(item);
                                if (debug) Debug.WriteLine($"Player: Removed {item.Type} from world");
                            }
                            else {
                                if (debug) Debug.WriteLine($"Player: Inventory full. {item.Type} remains in the world.");
                            }
                            break;
                        }
                        else {
                            if (debug) Debug.WriteLine("Player: Cannot pick up item, inventory is null");
                        }
                    }
                }
            }
        }
    }
}