using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal class Inventory : Component
    {
        public List<Item> items = new List<Item>();
        private int maxItems = 5; // Maximum inventory capacity
        private Player player;
        private TileMap tileMap;
        private Globals globals;
        private GameHUD gameHUD;

        public override void Start()
        {
            globals = Globals.Instance;
            player = GameObject.GetComponent<Player>();
            if (player == null)
            {
                Debug.WriteLine("Inventory: Player component not found.");
            }

            tileMap = globals._mapSystem?.Tilemap;
            if (tileMap == null)
            {
                Debug.WriteLine("Inventory: TileMap not found.");
            }

            gameHUD = GameObject.GetComponent<GameHUD>();
            if (gameHUD == null)
            {
                Debug.WriteLine("Inventory: GameHUD was NULL.");
            }
        }
        public override void Update(float deltaTime)
        {
            if (gameHUD == null) // since gamehud is created later, iterate until it is dound
            {
                gameHUD = globals._gameHUD;
                if (gameHUD == null) return;
            }
        }
        // adds an item to the inventory list with an inv. capacity check
        public void AddItem(Item item)
        {
            if (items.Count < maxItems)
            {
                items.Add(item);
                // update GameHUD inventory UI
                gameHUD.UpdateInventoryHUD(); // GAMEHUD: update inv slot UI (NULL)
                Debug.WriteLine($"Inventory: Added {item.Type} to inventory. Total items: {items.Count}");

                // Print all current inventory items
                Debug.WriteLine("Current Inventory: " + string.Join(", ", items.ConvertAll(i => i.Type.ToString())));
            }
            else {
                Debug.WriteLine("Inventory: Cannot add item. Inventory is full!");
            }
        }

        // checks if a specific item type exists in inventory
        public bool HasItem(ItemType type)
        {
            return items.Exists(item => item.Type == type);
        }
        // retrieves a specific item by type
        public Item GetItem(ItemType type)
        {
            return items.Find(item => item.Type == type);
        }
        // uses an item and removes it from inventory
        public void UseItem(int index, ItemType type)
        {
            Item item = GetItem(type);
            if (item != null)
            {
                item.Use(player, tileMap); // Call the Use method of the item
                items.Remove(item); // Remove the item from inventory after use
                Debug.WriteLine($"Inventory: item {type} used!");
                gameHUD?.UpdateInventoryHUD(); // GAMEHUD: update inv slot UI
            }
            else {
                Debug.WriteLine($"Inventory: Cannot use {type}. Item not in inventory.");
            }
        }
        public void UseInventoryItem(int index)
        {
            // Retrieve the item at the given index
            if (index < 0 || index >= items.Count)
            {
                Debug.WriteLine("Inventory: Invalid index.");
                return;
            }

            Item item = items[index];
            if (item != null)
            {
                UseItem(index, item.Type); // Call UseItem() with the correct item type
            }
            else {
                Debug.WriteLine($"Inventory: No item found at index {index}.");
            }
        }
        // returns all inventory items
        public List<Item> GetAllItems()
        {
            return items;
        }
        // removes a specific item from inventory
        public void RemoveItem(Item item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
                Debug.WriteLine($"Inventory: Removed {item.Type} from inventory");
                gameHUD?.UpdateInventoryHUD(); // GAMEHUD: update inv slot UI
            }
        }
    }
}

