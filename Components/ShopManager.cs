using System;
using System.Collections.Generic;
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

    }



}
