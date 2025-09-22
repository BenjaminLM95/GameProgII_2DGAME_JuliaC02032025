using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal class ShopManager : Component
    {
        // ---------- REFERENCES ---------- //
        Globals globals;

        // -----------VARIABLES ------------//
        public List<Shop> shops = new List<Shop>();
        public Shop currentShop = new Shop(); 

        public override void Start()
        {
            // null checks & component assignment
            globals = Globals.Instance; // globals
            AddAllShops();

            setShopsInTilemap();
        }

        public void AddAllShops() 
        {
            // Set all the shops, in this case there are three
            setShopOne();
            setShopTwo();
            setShopThree();
        }

        private void setShopOne() 
        {
            // Create the first shop which will contain: one potion, a fire scroll and a warpScroll
            Shop firstShop = new Shop();
            firstShop.itemInStock.Add(ItemType.HealthPotion);
            firstShop.itemInStock.Add(ItemType.FireScroll);
            firstShop.itemInStock.Add(ItemType.WarpScroll); 
            shops.Add(firstShop);

        }

        private void setShopTwo() 
        {
            // Create the second shop which will contain: one potion, a lightning scroll and a warpscroll
            Shop secondShop = new Shop();
            secondShop.itemInStock.Add(ItemType.HealthPotion);
            secondShop.itemInStock.Add(ItemType.LightningScroll);
            secondShop.itemInStock.Add(ItemType.WarpScroll);
            shops.Add(secondShop); 
        }

        private void setShopThree() 
        {
            //Create the third shop which will contain: a fire scroll, a lightning scroll and a warp scroll
            Shop thirdShop = new Shop();
            thirdShop.itemInStock.Add(ItemType.FireScroll);
            thirdShop.itemInStock.Add(ItemType.LightningScroll);
            thirdShop.itemInStock.Add(ItemType.WarpScroll);
            shops.Add(thirdShop);
        }

        public void closeShop() 
        {
            currentShop = null;
        }

        public void openShop(List<Shop> listShop, int index) 
        {
            closeShop();

            if (listShop[index] != null)
            currentShop = listShop[index];
        }

        public void setShopPosition(List<Shop> _listShops, int index, Vector2 pos) 
        {
            _listShops[index].shopPosition = pos;
        }

        public override void Draw(SpriteBatch spriteBatch) 
        {
            try
            {
                for(int i = 0; i < shops.Count; i++) 
                {
                    spriteBatch.Draw(shops[i].shopTexture, new Rectangle((int)shops[i].shopPosition.X, (int)shops[i].shopPosition.Y, 32, 32), 
                        Color.White);                  
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Items: Error drawing item: {ex.Message}");
            }
        }

        public void setShopsInTilemap() 
        {
            // put the shop in certain position
            setShopPosition(shops, 0, new Vector2(608, 224));
            setShopPosition(shops, 1, new Vector2(160, 320));
            setShopPosition(shops, 2, new Vector2(64, 96));
        }

        public void resetShops() 
        {
            for (int i = 0; i < shops.Count; i++) 
            {
                shops[i].itemInStock.Clear();
                AddAllShops();
            }
        }

    }



}
