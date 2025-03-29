using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using GameProgII_2DGame_Julia_C02032025.Components.Enemies;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal class GameHUD : Component
    {
        Globals globals;
        Combat combat;
        SpriteFont myFont;
        TileMap tileMap;
        Player player;

        // INVENTORY
        private List<Sprite> itemSlotSprites = new List<Sprite>();
        private Texture2D emptyInvTexture;
        private Inventory playerInventory;

        private Vector2 inventoryPosition;
        private int slotSize = 40; // Size of each inv slot
        private int spacing = 1;  // Space between inv slots

        public override void Start()
        {
            Debug.WriteLine("GameHUD: Start()");

            globals = globals ?? Globals.Instance;
            if (globals == null)
            {
                Debug.WriteLine("Enemy: globals is NULL!");
                throw new InvalidOperationException("GameHUD: Globals instance could not be initialized");
            }
            
            // Ensure player exists and has an inventory
            player = GameObject.GetComponent<Player>();
            playerInventory = GameObject.GetComponent<Inventory>();
            playerInventory = GameObject.FindObjectOfType<Inventory>();
            if (player != null)
            {
                Debug.WriteLine("GameHUD: Player gameobject found.");
            }
            if (playerInventory == null) {
                Debug.WriteLine("GameHUD: Player inventory not found.");
            }
            else if (playerInventory != null) {
                Debug.WriteLine("GameHUD: Player inventory found!");
            }

            // Get TileMap reference
            tileMap = globals._mapSystem.Tilemap;
            if (tileMap == null) {
                Debug.WriteLine("GameHUD: TileMap reference is NULL!");
            }

            // Initialize the empty inventory projSprite
            if (emptyInvTexture == null)
            {
                // Ensure the projSprite is loaded properly
                emptyInvTexture = Globals.content.Load<Texture2D>("emptyInvTexture"); 
                if (emptyInvTexture == null)
                {
                    Debug.WriteLine("GameHUD: emptyInvTexture is still null after loading!");
                    throw new InvalidOperationException("GameHUD: Failed to load empty inventory projSprite");
                }
            }

            // Initialize inventory slots
            for (int i = 0; i < 5; i++) {
                itemSlotSprites.Add(new Sprite { Texture = emptyInvTexture });
            }
        }
        public override void Update(float deltaTime) {                       
            UpdateInventoryHUD();
        }
        
        public void UpdateInventoryHUD(bool debug = false)
        {
            if(debug) Debug.WriteLine("GameHUD: UpdateInventoryHUD");

            if (playerInventory == null) 
            {
                if (debug) Debug.WriteLine("GameHUD: inventory reference is still NULL!");
                return;
            }

            for (int i = 0; i < itemSlotSprites.Count; i++)
            {
                var inventory = playerInventory.items;
                if (i < inventory.Count)
                {
                    itemSlotSprites[i].Texture = GetItemTexture(inventory[i]);
                }
                else
                {
                    itemSlotSprites[i].Texture = emptyInvTexture;
                }
            }
        }
        public void DrawInventoryHUD(bool debug = false)
        {
            // position inventory slot projSprite on the bottom of the screen (five of them next to each other)
            // draw the same way as turnindicator
            if (emptyInvTexture == null)
            {
                if (debug) Debug.WriteLine("GameHUD: empty inventory slot projSprite is NULL!");
                //return;
            }

            Vector2 position = inventoryPosition;
            
            for (int i = 0; i < itemSlotSprites.Count; i++)
            {
                if (debug) { // Log the position of the slot being drawn
                    Debug.WriteLine($"Drawing slot {i + 1} at position: {position}");
                }

                Globals.spriteBatch.Draw(
                    itemSlotSprites[i].Texture,
                    position,
                    Color.White
                );

                if (debug) { // Log projSprite information for the current slot 
                    Debug.WriteLine($"Slot {i + 1} projSprite: {itemSlotSprites[i].Texture?.Name ?? "No Texture"}");
                }

                // Draw slot number above the slot
                string slotNumber = (i + 1).ToString(); // Number from 1 to 5
                Vector2 numberPosition = new Vector2(position.X + slotSize / 2, position.Y - 20); // Adjust position

                //Globals.spriteBatch.DrawString(myFont, slotNumber, numberPosition, Color.White); // font not being recognized?
                
                if (debug) { // Log the number being drawn
                    Debug.WriteLine($"Drawing slot number {slotNumber} at position: {numberPosition}");
                }
                position.X += slotSize + spacing; // Move to the next slot
            }
            // if an item is used, replace the projSprite with an empty one
        }
        private Texture2D GetItemTexture(Item item)
        {
            if (tileMap == null || item == null) return emptyInvTexture;

            return item.Type switch // change to alphabet
            {
                ItemType.HealthPotion => tileMap.healthPotionTexture, // get textures from TileMap
                ItemType.FireScroll => tileMap.fireScrollTexture,
                ItemType.LightningScroll => tileMap.lightningScrollTexture,
                ItemType.WarpScroll => tileMap.warpScrollTexture,
                _ => emptyInvTexture,
            };
        }
        public void DrawDamageNumbers()
        {
            // take damage that the entity recieved and draw it above their head
            // + take any healing the player does and draw it green above their head
        }
    }
}
