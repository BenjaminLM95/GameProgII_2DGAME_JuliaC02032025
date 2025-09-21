using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal class Shop
    {
        public List<ItemType> itemInStock = new List<ItemType>();

        public bool open = false; 

        public void buyItem(ItemType type) 
        {
            // Check if the item is in stock
            if (itemInStock.Contains(type))
            {
                // Get the index of the item
                int i = itemInStock.IndexOf(type);

                // Check if player has enough money
                if (Globals.Instance._player.currency >= getBuyPrice(type))
                {
                    // proceed with the purchase
                    Globals.Instance._player.currency -= getBuyPrice(type);
                    // Remove the item from the shop
                    itemInStock.Remove(itemInStock[i]);
                    // Add the item to the player inventory
                    // TODO

                }
            }
        }
        
        public void AddItemInStock(ItemType type) 
        {
            itemInStock.Add(type);
        }


        public int getBuyPrice(ItemType itemType) 
        {
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

    }
}
