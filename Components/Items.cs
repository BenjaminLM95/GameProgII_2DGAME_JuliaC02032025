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
        WarpScroll
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
                    //WarpPlayer(player, tileMap);
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
                healthSystem.ModifyHealth(healAmount);
                Debug.WriteLine($"Items: Player healed for {healAmount} HP");
            }
            else {
                Debug.WriteLine("Items: Error HealthSystem component not found on player");
            }
        }
        // /*
        private void CastFireball(Player player, TileMap tileMap)
        {
            Vector2 direction = GetPlayerFacingDirection();
            Vector2 projectilePosition = player.LastMovementDirection + direction * 32; // one tile in front of player

            // Check for enemy collision along the path
            while (IsValidTile(projectilePosition, tileMap))
            {
                Debug.WriteLine("Items: found valid tile for CastFireball");
                // check for enemy at this position and apply damage
                // check the Enemies component
                projectilePosition += direction * 32;
            }
        }
        // */
        private void CastLightning(TileMap tileMap)
        {
            // find and damage all visible enemies
            // access to the Enemies component
            Debug.WriteLine("Items: Lightning Scroll: Damaging all visible enemies");
        }
        /*
        private void WarpPlayer(Player player, TileMap tileMap)
        {
            Vector2 randomEmptyTile = tileMap.GetRandomEmptyTile();
            if (randomEmptyTile != new Vector2(-1, -1))
            {
                player.Position = randomEmptyTile;
                Debug.WriteLine("Player warped to a new location");
            }
        }
        */

        private Vector2 GetPlayerFacingDirection()
        {
            // need specific logic for player movement/input system
            // returns a direction based on last movement
            return Vector2.UnitX; 
        }

        private bool IsValidTile(Vector2 position, TileMap tileMap)
        {
            int x = (int)position.X / 32;
            int y = (int)position.Y / 32;
            Sprite tile = tileMap.GetTileAt(x, y);
            return tile != null && tile.Texture == tileMap.floorTexture;
        }
    }

    internal class Items : Component
    {
        private Globals globals;
        private TileMap tileMap;
        public List<Item> itemPool = new List<Item>();
        public List<Item> spawnedItems = new List<Item>();

        private ItemType currentItemType;

        public override void Start()
        {
            globals = Globals.Instance;

            if (globals == null)
            {
                Debug.WriteLine("Items: Globals is null in Items.Start()");
                return;
            }

            tileMap = globals._mapSystem?.Tilemap;

            if (tileMap == null)
            {
                Debug.WriteLine("Items: TileMap is null in Items.Start()");
                return;
            }

            InitializeItemPool();

            SpawnItems(4); // spawn 4 random items
        }

        private void InitializeItemPool()
        {
            if (tileMap == null)
            {
                Debug.WriteLine("Items: TileMap is null during item pool initialization");
                return;
            }

            // ensure textures are loaded before creating items
            Texture2D healthPotionTexture = tileMap.healthPotionTexture ?? LoadTexture("healthPotion");
            Texture2D fireScrollTexture = tileMap.fireScrollTexture ?? LoadTexture("fireScroll");
            Texture2D lightningScrollTexture = tileMap.lightningScrollTexture ?? LoadTexture("lightningScroll");
            Texture2D warpScrollTexture = tileMap.warpScrollTexture ?? LoadTexture("warpScroll");

            itemPool.Clear(); // clear existing items if any
            itemPool.Add(new Item(ItemType.HealthPotion, healthPotionTexture));
            itemPool.Add(new Item(ItemType.FireScroll, fireScrollTexture));
            itemPool.Add(new Item(ItemType.LightningScroll, lightningScrollTexture));
            itemPool.Add(new Item(ItemType.WarpScroll, warpScrollTexture));
        }

        // load texture if not already loaded
        private Texture2D LoadTexture(string textureName)
        {
            try
            {
                return Globals.content.Load<Texture2D>(textureName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Items: Failed to load texture {textureName}: {ex.Message}");
                return null;
            }
        }
        public void InitializeItemType(ItemType type)
        {
            currentItemType = type;
            // Optionally, you could initialize the specific item in the item pool
            Item specificItem = itemPool.FirstOrDefault(i => i.Type == type);
            if (specificItem != null)
            {
                spawnedItems.Add(specificItem);
            }
        }

        public void SpawnItems(int amount)
        {
            spawnedItems.Clear();
            Random random = new Random();

            for (int i = 0; i < amount; i++)
            {
                // randomly select an item type
                Debug.WriteLine($"Items: spawned");
                Item itemToSpawn = itemPool[random.Next(itemPool.Count)];
                PlaceItem(itemToSpawn);
            }
        }

        private void PlaceItem(Item item)
        {
            Vector2 randomTile = globals._mapSystem.GetRandomEmptyTile();
            if (randomTile != new Vector2(-1, -1))
            {
                item.Position = randomTile;
                spawnedItems.Add(item);
                Debug.WriteLine($"Items: Spawned {item.Type} at {randomTile}");
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (spriteBatch == null)
            {
                Debug.WriteLine("SpriteBatch is null in Items.Draw()");
                return;
            }

            foreach (var item in spawnedItems)
            {
                if (item.Texture == null)
                {
                    Debug.WriteLine($"Items: Texture is null for item of type {item.Type}");
                    continue;
                }

                try
                {
                    spriteBatch.Draw(item.Texture, item.Position, Color.White);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Items: Error drawing item {item.Type}: {ex.Message}");
                }
            }
        }
        public void ClearItems()
        {
            foreach (var item in spawnedItems)
            {
                globals._scene.RemoveGameObject(item);
            }

            spawnedItems.Clear();
            Debug.WriteLine("Items: All items cleared from the scene.");
        }

        public void UseItem(Player player, Item item)
        {
            if (spawnedItems.Contains(item))
            {
                item.Use(player, tileMap);
                spawnedItems.Remove(item);
            }
        }
        // /*
        public List<Item> GetItems()
        {
            // Remove any items with null GameObjects
            spawnedItems.RemoveAll(e => e == null);
            return spawnedItems;
        }
        // */
    }
}
