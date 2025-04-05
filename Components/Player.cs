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
    internal class Player : Component, ITurnTaker
    {
        Globals globals;
        private TileMap tileMap;
        private HealthSystem healthSystem;
        private Sprite playerSprite;
        private Inventory inventory;
        private TurnManager turnManager; 

        // ---------- VARIABLES ---------- //

        private float speed = 300f;
        private int tileSize = 32;
        private int spriteScale = 1;

        // Turn based combat
        private bool canMove = false;
        public bool hasMovedThisTurn = false;
        public bool playerMovedOntoEnemyTile { get; private set; }

        public Vector2 LastMovementDirection { get; private set; } = Vector2.UnitX; // Default facing right
        private KeyboardState previousKeyboardState;

        public Player() { }
        public Player(TileMap tileMap)
        {
            this.tileMap = tileMap;
        }

        // ---------- METHODS ---------- //

        // Initializes the player by finding the map system and tile map.
        public override void Start()
        {
            Debug.WriteLine("Player: START()");
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
            StartTurn(turnManager);
        }
        public void StartTurn(TurnManager manager)
        {
            canMove = true;
            turnManager = manager;
            Debug.WriteLine("Player: Turn started.");
        }
        public void ResetTurn()
        {
            canMove = false;
            hasMovedThisTurn = false;
            Debug.WriteLine("Player: Turn reset.");
        }
        public void EndPlayerTurn()
        {
            canMove = false;
            if (turnManager != null)
            {
                turnManager.EndTurn();
                Debug.WriteLine("Player: Turn ended.");
            }
        }
        
        public override void Update(float deltaTime)
        {
            if (!canMove || hasMovedThisTurn)
                return; // Disallow movement if it's not player's turn or they've already moved

            if (hasMovedThisTurn || turnManager == null) return;

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
            previousKeyboardState = Keyboard.GetState();
        }
        private void ReadInput(bool debug = true)
        {
            // Input
            Vector2 currentPos = GameObject.Position;
            Vector2 targetPos = currentPos;
            bool moved = false;
            Vector2 movementDirection = Vector2.Zero;

            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (IsNewKeyPress(currentKeyboardState, previousKeyboardState, Keys.W))
            {
                if(debug) Debug.WriteLine($"Player: moving UP");
                targetPos.Y -= tileSize;
                moved = true;
                movementDirection = -Vector2.UnitY;
            }
            if (IsNewKeyPress(currentKeyboardState, previousKeyboardState, Keys.A))
            {
                if (debug) Debug.WriteLine($"Player: moving LEFT");
                targetPos.X -= tileSize;
                moved = true;
                movementDirection = -Vector2.UnitX;
            }
            if (IsNewKeyPress(currentKeyboardState, previousKeyboardState, Keys.S))
            {
                if (debug) Debug.WriteLine($"Player: moving DOWN");
                targetPos.Y += tileSize;
                moved = true;
                movementDirection = Vector2.UnitY;
            }
            if (IsNewKeyPress(currentKeyboardState, previousKeyboardState, Keys.D))
            {
                if (debug) Debug.WriteLine($"Player: moving RIGHT");
                targetPos.X += tileSize;
                moved = true;
                movementDirection = Vector2.UnitX;
            }
            
            for (int i = 0; i < 5; i++) // Check if a number key (1-5) is pressed to use an item
            {
                Keys key = (Keys)((int)Keys.D1 + i); // Maps 1-5 keys

                if (IsNewKeyPress(currentKeyboardState, previousKeyboardState, key))
                {
                    if (debug) Debug.WriteLine($"Player: reading keys 1-5"); 
                    inventory.UseInventoryItem(i);
                }
            }

            if (moved) 
            {
                // Prevent illegal movement more than 32 pixels
                Vector2 difference = targetPos - currentPos;
                if (Math.Abs(difference.X) > tileSize || Math.Abs(difference.Y) > tileSize)
                {
                    Debug.WriteLine("Player: THAT'S ILLEGAL! Only 1 tile (32px) allowed per turn.");
                    return;
                }
                if (difference.X != 0 && difference.Y != 0)
                {
                    Debug.WriteLine("Player: Diagonal movement not allowed.");
                    return;
                }
                if (movementDirection != Vector2.Zero) // Update last movement direction
                {
                    LastMovementDirection = movementDirection;
                }
                // Convert target position to tile coordinates
                Point targetTilePos = GetTileCoordinates(targetPos);

                // Check obstacle
                if (IsObstacle(targetTilePos))
                {
                    Debug.WriteLine("Player: Can't move, tile is an obstacle.");
                    return;
                }

                if (IsExit(targetTilePos)) // if tile is exit, Load Next Level
                {
                    GameObject.Position = targetPos;
                    hasMovedThisTurn = true;
                    
                    globals._mapSystem.LoadNextLevel(); // gen rand next level
                    
                    EndPlayerTurn(); // Advance turn after moving to exit
                    return;
                }

                if (IsItem(targetTilePos)) // if tile is an Item, add it to inventory
                {
                    GameObject.Position = targetPos;
                    if (debug) Debug.WriteLine($"Player: moved to ITEM position - {targetPos}");
                    hasMovedThisTurn = true;

                    PickUpItem(targetTilePos); // Pick up the item at this position

                    EndPlayerTurn();
                    if (debug) Debug.WriteLine("Current Inventory: "
                        + string.Join(", ", inventory.items.ConvertAll(i => i.Type.ToString())));
                    return;
                }

                // Normal move
                GameObject.Position = targetPos;
                if (debug) Debug.WriteLine($"Player: moved to position - {targetPos}");
                hasMovedThisTurn = true;
                EndPlayerTurn();
            }
        }
        private bool IsNewKeyPress(KeyboardState current, KeyboardState previous, Keys key)
        {
            return current.IsKeyDown(key) && previous.IsKeyUp(key);
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
                    EndPlayerTurn();
                    break;
                }
            }
        }
        public void Attack(Enemy enemy, int amountdmg)
        {
            Debug.WriteLine($"Player: Attacked enemy for {amountdmg} dmg");
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