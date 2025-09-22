using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal class Shop : Component
    {
        public List<ItemType> itemInStock = new List<ItemType>();

        public bool open = false;

        // References
        private TileMap tileMap;
        private Globals globals;
        private Inventory inventory;
        public Texture2D shopTexture { get; private set; }

        public Vector2 shopPosition; 

        public Shop() 
        {
            globals = Globals.Instance;
            tileMap = globals._mapSystem?.Tilemap;            
            shopTexture = tileMap.shopTexture ?? LoadTexture("ShopImg"); 
        }

        public override void Start() 
        {
            inventory = GameObject.FindObjectOfType<Inventory>(); // inventory
        }


        public void buyItem(ItemType type) 
        {
            if (inventory == null) 
            {
                inventory = GameObject.FindObjectOfType<Inventory>(); // inventory
            }

            // Check if the item is in stock
            if (itemInStock.Contains(type))
            {
                if(type.ToString() != "Sold" && type.ToString() != "None") //Check if the item is not a sold item or none existence item
                { 
                // Get the index of the item
                    int i = itemInStock.IndexOf(type);

                    // Check if player has enough money
                    if (Globals.Instance._player.currency >= getBuyPrice(type))
                    {
                        // proceed with the purchase
                        Globals.Instance._player.currency -= getBuyPrice(type);
                        // Player gets the item
                        Item _newItem = getItem(type);
                        inventory.AddItem(_newItem);
                        // Remove the item from the shop
                        itemInStock[i] = ItemType.Sold;
                        // Add the item to the player inventory
                        
                    }

                }
            }
        }
        
        public void AddItemInStock(ItemType type) 
        {
            itemInStock.Add(type);
        }


        public int getBuyPrice(ItemType itemType) 
        {
            // Get the price depend of which item it is
            int itemPrice; 
            switch (itemType)
            {
                case ItemType.HealthPotion:
                    itemPrice = 10; 
                    break;
                case ItemType.FireScroll:
                    itemPrice = 18; 
                    break;
                case ItemType.LightningScroll:
                    itemPrice = 23; 
                    break;
                case ItemType.WarpScroll:
                    itemPrice = 15; 
                    break;
                default:
                    itemPrice = 0;
                    break; 
            }

            return itemPrice;
        }

        public int getSellPrice(ItemType itemType) 
        {
            // get the sell price depends of which item it is
            int itemPrice;
            switch (itemType)
            {
                case ItemType.HealthPotion:
                    itemPrice = 6;
                    break;
                case ItemType.FireScroll:
                    itemPrice = 10;
                    break;
                case ItemType.LightningScroll:
                    itemPrice = 12;
                    break;
                case ItemType.WarpScroll:
                    itemPrice = 8;
                    break;
                default:
                    itemPrice = 0;
                    break;
            }

            return itemPrice;
        }

        public void sellItem(ItemType type)
        {
            Globals.Instance._player.currency += getSellPrice(type);
            AddItemInStock(type); 
        }

        private Texture2D LoadTexture(string textureName, bool debug = false)
        {
            try
            {
                if (debug) Debug.WriteLine($"Items: Attempting to load projSprite '{textureName}'");
                return Globals.content.Load<Texture2D>(textureName);
            }
            catch (Exception ex)
            {
                if (debug) Debug.WriteLine($"Items: Failed to load projSprite {textureName}: {ex.Message}");
                return null;
            }
        }

        private Item getItem(ItemType itemType) 
        {
            Texture2D itemTexture;
           
            switch (itemType)
            {
                case ItemType.HealthPotion:
                    itemTexture = Globals.content.Load<Texture2D>("healthPotion");                    
                    break;
                case ItemType.FireScroll:
                    itemTexture = Globals.content.Load<Texture2D>("fireScroll");
                    break;
                case ItemType.LightningScroll:
                    itemTexture = Globals.content.Load<Texture2D>("lightningScroll");
                    break;
                case ItemType.WarpScroll:
                    itemTexture = Globals.content.Load<Texture2D>("warpScroll");
                    break;                
                default:
                    itemTexture = Globals.content.Load<Texture2D>("emptyInvTexture");
                    break;
            }

            return new Item(itemType, itemTexture);
        }

    }
}
