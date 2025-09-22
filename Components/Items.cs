using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    public enum ItemType
    {
        HealthPotion,
        FireScroll,
        LightningScroll,
        WarpScroll,
        Sold,
        None
    }

    internal class Item : GameObject
    {
        public ItemType Type { get; private set; }
        public Texture2D Texture { get; private set; }

        public Item(ItemType type, Texture2D texture)
        {
            Type = type;
            Texture = texture;
        }

        public void Use(Player player, TileMap tileMap)
        {
            switch (Type)
            {
                case ItemType.HealthPotion:
                    HealPlayer(player);
                    break;
                case ItemType.FireScroll:
                    CastFireball(player, tileMap);
                    break;
                case ItemType.LightningScroll:
                    CastLightning(tileMap);
                    break;
                case ItemType.WarpScroll:
                    WarpPlayer(player, tileMap);
                    break;
                case ItemType.Sold:
                    break;
                case ItemType.None:
                    break;
            }
        }

        private void HealPlayer(Player player)
        {
            HealthSystem healthSystem = player.GameObject.GetComponent<HealthSystem>();

            if (healthSystem != null)
            {
                int healAmount = 20;
                // Use the ModifyHealth method to heal the player
                healthSystem.ModifyHealth(+healAmount);
                Debug.WriteLine($"Items: HEALTHPOTION - Player healed for {healAmount} HP, current health: {healthSystem.CurrentHealth}");
            }
            else {
                Debug.WriteLine("Items: Error HealthSystem component not found on player");
            }
        }
        
        private void CastFireball(Player player, TileMap tileMap)
        {
            Debug.WriteLine("Items: CastFireball() called");

            Vector2 direction = GetPlayerFacingDirection(player); // get the direction the player's facing 
            Vector2 projectilePosition = player.GameObject.Position + direction * 32; // one tile in front of player

            // create a new projectile GameObject
            GameObject fireballObj = new GameObject();
            fireballObj.Position = projectilePosition;

            // add a sprite component with the fireball projSprite
            Sprite fireballSprite = new Sprite();
            fireballObj.AddComponent(fireballSprite);
            fireballSprite.LoadSprite("player_FireballProj");

            // add a movement component or script to handle the projectile movement
            Projectile fireball = new Projectile(fireballSprite, direction, 200f, player, tileMap);
            fireballObj.AddComponent(fireball);

            // add the fireball to the scene
            Globals.Instance._scene.AddGameObject(fireballObj);

            Debug.WriteLine($"Items: FIREBALLSCROLL - Created fireball projectile moving in direction {direction}");
        }
        // find and damage all visible enemies
        private void CastLightning(TileMap tileMap)
        {
            Debug.WriteLine("Items: CastLightning() called");
            
            // ensure there is at least one enemy before calling GetEnemies()
            Enemy firstEnemy = GameObject.FindObjectOfType<Enemy>();
            if (firstEnemy == null)
            {
                Debug.WriteLine("Items: LIGHTNINGSCROLL - No enemies found to damage");
                return; // Exit early to prevent errors
            }

            // get all current enemies from the scene
            List<Enemy> enemies = new List<Enemy>(firstEnemy.GetEnemies());

            // attack each enemy in the list
            if (enemies.Count > 0)
            {
                foreach (Enemy enemy in enemies)
                {
                    if (enemy != null && enemy.GameObject != null) // check if enemy is still valid in the scene
                    {
                        Globals.Instance._player.Attack(enemy, 50);
                        Debug.WriteLine($"Items: LIGHTNINGSCROLL - Damaged enemy at position {enemy.GameObject.Position}");
                    }
                }
                Debug.WriteLine($"Items: LIGHTNINGSCROLL - Damaged {enemies.Count} enemies");
            }
            else {
                Debug.WriteLine("Items: LIGHTNINGSCROLL - No enemies found to damage");
            }
        }

        // CUSTOM ITEM: warps the player to any random floor tile
        private void WarpPlayer(Player player, TileMap tileMap)
        {
            Vector2 randomEmptyTile = Globals.Instance._mapSystem.GetRandomEmptyTile();

            if (randomEmptyTile != new Vector2(-1, -1))
            {
                player.GameObject.Position = randomEmptyTile;
                Debug.WriteLine("Player warped to a new location");
            }
        }
        

        private Vector2 GetPlayerFacingDirection(Player player)
        {
            // get the player's last movement direction
            Vector2 lastDirection = player.LastMovementDirection;

            // normalize to only allow cardinal directions (up, down, left, right)
            if (Math.Abs(lastDirection.X) > Math.Abs(lastDirection.Y)) {
                // horizontal movement
                return lastDirection.X > 0 ? Vector2.UnitX : -Vector2.UnitX;
            }
            else {
                // vertical movement
                return lastDirection.Y > 0 ? Vector2.UnitY : -Vector2.UnitY;
            }
        }
    }

    internal class Items : Component
    {
        private Globals globals;
        private TileMap tileMap;
        public List<Item> itemPool = new List<Item>();
        public List<Item> spawnedItems = new List<Item>();

        private bool initialized = false;
        private int itemsToSpawn = 10;

        private ItemType currentItemType;

        public override void Start()
        {
            globals = Globals.Instance;

            if (globals == null) {
                Debug.WriteLine("Items: Globals is null in Items.Start()");
                return;
            }

            tileMap = globals._mapSystem?.Tilemap;
            // wait for update to recognize an existing tilemap before spawning items
            Debug.WriteLine("Items: Component started, waiting for TileMap to be ready");
        }
        public override void Update(float deltaTime)
        {
            if (initialized) return; // If item is already initialized skip

            if (tileMap == null) // Try to get the TileMap
            {
                tileMap = globals._mapSystem?.Tilemap;
                
                if (tileMap == null)
                    return; // If TileMap is still null return and try again next frame
            }

            // TileMap is now available
            Debug.WriteLine("Items: TileMap is now available, initializing items");
            InitializeItemPool();
            SpawnItems(itemsToSpawn);
            initialized = true;
        }

        private void InitializeItemPool(bool debug = false)
        {
            if (tileMap == null)
            {
                if(debug) Debug.WriteLine("Items: TileMap is null during item pool initialization");
                return;
            }

            if (debug) Debug.WriteLine("Items: Initializing item pool");

            // ensure textures are loaded before creating items
            Texture2D healthPotionTexture = tileMap.healthPotionTexture ?? LoadTexture("healthPotion");
            Texture2D fireScrollTexture = tileMap.fireScrollTexture ?? LoadTexture("fireScroll");
            Texture2D lightningScrollTexture = tileMap.lightningScrollTexture ?? LoadTexture("lightningScroll");
            Texture2D warpScrollTexture = tileMap.warpScrollTexture ?? LoadTexture("warpScroll");

            // Debug the loaded textures
            if (debug) Debug.WriteLine($"Items: Health potion projSprite loaded: {healthPotionTexture != null}");
            if (debug) Debug.WriteLine($"Items: Fire scroll projSprite loaded: {fireScrollTexture != null}");
            if (debug) Debug.WriteLine($"Items: Lightning scroll projSprite loaded: {lightningScrollTexture != null}");
            if (debug) Debug.WriteLine($"Items: Warp scroll projSprite loaded: {warpScrollTexture != null}");

            itemPool.Clear(); // clear existing items if any
            itemPool.Add(new Item(ItemType.HealthPotion, healthPotionTexture));
            itemPool.Add(new Item(ItemType.FireScroll, fireScrollTexture));
            itemPool.Add(new Item(ItemType.LightningScroll, lightningScrollTexture));
            itemPool.Add(new Item(ItemType.WarpScroll, warpScrollTexture));
            // check the initialized item pool
            if (debug) Debug.WriteLine($"Items: Item pool initialized with {itemPool.Count} item types");
        }

        // load projSprite if not already loaded
        private Texture2D LoadTexture(string textureName, bool debug = false)
        {
            try {
                if (debug) Debug.WriteLine($"Items: Attempting to load projSprite '{textureName}'");
                return Globals.content.Load<Texture2D>(textureName);
            }
            catch (Exception ex) {
                if (debug) Debug.WriteLine($"Items: Failed to load projSprite {textureName}: {ex.Message}");
                return null;
            }
        }
        
        public void SpawnItems(int amount, bool debug = false)
        {
            if (tileMap == null)
            {
                if (debug) Debug.WriteLine("Items: Cannot spawn items, TileMap is null");
                itemsToSpawn = amount; // save for when TileMap is available
                return;
            }

            if (debug) Debug.WriteLine($"Items: Attempting to spawn {amount} items");
            spawnedItems.Clear();
            Random random = new Random();

            for (int i = 0; i < amount; i++)
            {
                if (itemPool.Count == 0) // randomly select an item type
                {
                    if (debug) Debug.WriteLine("Items: Cannot spawn items, item pool is empty");
                    return;
                }

                Item itemToSpawn = itemPool[random.Next(itemPool.Count)];
                PlaceItem(itemToSpawn);
            }
            if (debug) Debug.WriteLine($"Items: Successfully spawned {spawnedItems.Count} items");
        }

        private void PlaceItem(Item item, bool debug = false)
        {
            if (globals._mapSystem == null) {
                if (debug) Debug.WriteLine("Items: Cannot place item, MapSystem is null");
                return;
            }

            Vector2 randomTile = globals._mapSystem.GetRandomEmptyTile();
            if (randomTile != new Vector2(-1, -1))
            {
                // create a clone of the item to avoid duplicates
                Item newItem = new Item(item.Type, item.Texture);
                newItem.Position = randomTile;
                spawnedItems.Add(newItem);
                if (debug) Debug.WriteLine($"Items: Spawned {item.Type} at {randomTile}");
            }
            else {
                if (debug) Debug.WriteLine("Items: Failed to find empty tile for item placement");
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (spriteBatch == null) {
                Debug.WriteLine("SpriteBatch is null in Items.Draw()");
                return;
            }

            foreach (var item in spawnedItems)
            {
                if (item == null)
                    continue;

                if (item.Texture == null) {
                    Debug.WriteLine($"Items: Texture is null for item of type {item.Type}");
                    continue;
                }

                try {
                    spriteBatch.Draw(item.Texture, item.Position, Color.White);
                }
                catch (Exception ex) {
                    Debug.WriteLine($"Items: Error drawing item {item.Type}: {ex.Message}");
                }
            }
        }
      
        public List<Item> GetItems()
        {
            // Remove any items with null GameObjects
            spawnedItems.RemoveAll(e => e == null);
            return spawnedItems;
        }
    }
}
