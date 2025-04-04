using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Xna.Framework.Input;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal class GameHUD : Component
    {
        // ---------- REFERENCES ---------- //
        Globals globals;
        Combat combat;
        SpriteFont myFont; // FONT
        TileMap tileMap;
        HealthSystem healthSystem;
        Player player;

        // ---------- VARIABLES ---------- //
        // INVENTORY
        private List<Sprite> itemSlotSprites = new List<Sprite>();
        private Texture2D emptyInvTexture;
        private Inventory playerInventory;

        private Texture2D blankButton;
        private Texture2D pressedButton;
        private Texture2D menuBackground;

        private Vector2 textPosition; // FONT POS
        private Vector2 inventoryPosition;
        private int slotSize = 40; // Size of each inv slot
        private int spacing = 1;  // Space between inv slots
        public bool isMenuActive = true; // start showing the menu

        // button pos and activation state
        private Rectangle playButtonBounds;
        private Rectangle quitButtonBounds;
        private bool playButtonActive = true;
        private bool quitButtonActive = true;
        private bool buttonWasReleased = true;

        // ---------- METHODS ---------- //
        public override void Start()
        {
            Globals.Instance._gameHUD = this;

            // load & draw the font like a texture
            myFont = Globals.content.Load<SpriteFont>("Minecraft"); // loading font

            menuBackground = Globals.content.Load<Texture2D>("background");
            blankButton = Globals.content.Load<Texture2D>("blankButton"); // loading blank button texutre
            pressedButton = Globals.content.Load<Texture2D>("pressedButton"); // loading pressed button texutre

            Debug.WriteLine("GameHUD: Start()");

            globals = globals ?? Globals.Instance;
            if (globals == null) {
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
            // Get HealthSystem reference
            healthSystem = globals._healthSystem;
            if (healthSystem == null)
            {
                Debug.WriteLine("GameHUD: healthSystem reference is NULL!");
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

            // Initialize button bounds
            playButtonBounds = new Rectangle(375, 100, 200, 50);
            quitButtonBounds = new Rectangle(375, 150, 200, 50);
        }
        public override void Update(float deltaTime) 
        {                       
            UpdateInventoryHUD();

            if (isMenuActive)
            {
                Globals.TimeScale = 0f;
                HandleButtonInteractions();
            }
        }

        // checks the player's inventory for items, fills with matching item texture when called, empty if used
        public void UpdateInventoryHUD(bool debug = false)
        {
            if(debug) Debug.WriteLine("GameHUD: UpdateInventoryHUD");

            if (playerInventory == null) {
                if (debug) Debug.WriteLine("GameHUD: inventory reference is still NULL!");
                return;
            }

            for (int i = 0; i < itemSlotSprites.Count; i++)
            {
                var inventory = playerInventory.items;
                if (i < inventory.Count) {
                    itemSlotSprites[i].Texture = GetItemTexture(inventory[i]);
                }
                else {
                    itemSlotSprites[i].Texture = emptyInvTexture;
                }
            }
        }
        public void DrawInventoryHUD(bool debug = false)
        {
            // position inventory slot projSprite on the bottom of the screen (five of them next to each other)
            if (emptyInvTexture == null)
            {
                if (debug) Debug.WriteLine("GameHUD: empty inventory slot projSprite is NULL!");
            }

            Vector2 position = inventoryPosition;
            
            for (int i = 0; i < itemSlotSprites.Count; i++)
            {
                if (debug) { // log the position of the slot being drawn
                    Debug.WriteLine($"Drawing slot {i + 1} at position: {position}");
                }

                Globals.spriteBatch.Draw(
                    itemSlotSprites[i].Texture,
                    position,
                    Color.White
                );

                if (debug) { // log projSprite information for the current slot 
                    Debug.WriteLine($"Slot {i + 1} projSprite: {itemSlotSprites[i].Texture?.Name ?? "No Texture"}");
                }

                // draw slot number above the slot
                string slotNumber = (i + 1).ToString(); // number from 1 to 5
                Vector2 numberPosition = new Vector2(position.X + slotSize / 2, position.Y - 20); // adjust position
                
                if (debug) { // log the number being drawn
                    Debug.WriteLine($"Drawing slot number {slotNumber} at position: {numberPosition}");
                }
                position.X += slotSize + spacing; // move to the next slot
            }
        }

        // returns texture from TileMap depending on item picked up
        private Texture2D GetItemTexture(Item item)
        {
            if (tileMap == null || item == null) return emptyInvTexture;

            return item.Type switch 
            {
                ItemType.HealthPotion => tileMap.healthPotionTexture, 
                ItemType.FireScroll => tileMap.fireScrollTexture,
                ItemType.LightningScroll => tileMap.lightningScrollTexture,
                ItemType.WarpScroll => tileMap.warpScrollTexture,
                _ => emptyInvTexture,
            };
        }

        public string DrawFont(string text, Vector2 position)
        {
            Globals.spriteBatch.DrawString(
                        myFont, text, position, Color.White);
            return text;
        }
        public void DrawLevelFont(Vector2 position)
        {
            //levelNumber
            int level = Globals.Instance._mapSystem.levelNumber;
            DrawFont("Level: "+level, position);
        }
        public void DrawHealth(Vector2 position) // (add playerObject?)
        {
            //health 
            //player.GameObject.GetComponent<HealthSystem>()?.ModifyHealth(-10);
            //int health = healthSystem.CurrentHealth;
            int health = 100;
            DrawFont("Player Health: " + health, position);
        }

        private void DrawButton(Vector2 position, string text)
        {
            // draw single button, can put text on it. takes player's mouse position
            // add functionality to draw text on the button from here? 
            Globals.spriteBatch.Draw(
                        blankButton, position, Color.White);

            float buttonmidX = position.X + 16; // button sprite X is 64
            float buttonmidY = position.Y + 8; // button sprite Y is 32

            Vector2 textPos = new Vector2(buttonmidX, buttonmidY); // centering text on button
            // drawing text on the button
            DrawFont(text, textPos);

        }

        public void DrawScreen()
        {
            // only draw buttons if they're active
            if (isMenuActive)
            {
                Globals.spriteBatch.Draw(menuBackground, Vector2.Zero, Color.White);

                Vector2 playButtonPos = new Vector2(playButtonBounds.X, playButtonBounds.Y);
                Vector2 quitButtonPos = new Vector2(quitButtonBounds.X, quitButtonBounds.Y);

                // only draw buttons if they're active
                if (playButtonActive)
                {
                    MouseState mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
                    Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);

                    if (playButtonBounds.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        // draw pressed texture for Play
                        Globals.spriteBatch.Draw(pressedButton, playButtonPos, Color.White);
                    }
                    else {
                        DrawButton(playButtonPos, "PLAY"); // draw Play
                    }
                }

                if (quitButtonActive)
                {
                    MouseState mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
                    Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);

                    if (quitButtonBounds.Contains(mousePos) && mouseState.LeftButton == ButtonState.Pressed)
                    {
                        // draw pressed texture for Quit
                        Globals.spriteBatch.Draw(pressedButton, quitButtonPos, Color.White);
                    }
                    else {
                        DrawButton(quitButtonPos, "QUIT"); // draw Quit
                    }
                }
            }
        }
        private void ClearScreen()
        {
            // Deactivate all the buttons and the menu screen background
            playButtonActive = false;
            quitButtonActive = false;
            isMenuActive = false;
        }

        private void HandleButtonInteractions()
        {
            MouseState mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
            bool isLeftButtonPressed = mouseState.LeftButton == ButtonState.Pressed;
            bool isLeftButtonReleased = mouseState.LeftButton == ButtonState.Released;

            // Reset button released state when mouse button is released
            if (isLeftButtonReleased)
            {
                buttonWasReleased = true;
            }

            // Check Play button
            if (playButtonActive && playButtonBounds.Contains(mousePos))
            {
                if (isLeftButtonPressed && buttonWasReleased)
                {
                    // Play button pressed
                    isMenuActive = false;
                    playButtonActive = false;
                    buttonWasReleased = false;
                    Debug.WriteLine("GameHUD: Play button pressed!");
                    
                    Globals.TimeScale = 1f; // unpauses the game
                }
            }

            // Check Quit button
            if (quitButtonActive && quitButtonBounds.Contains(mousePos))
            {
                if (isLeftButtonPressed && buttonWasReleased)
                {
                    // Quit button pressed
                    quitButtonActive = false;
                    buttonWasReleased = false;
                    Debug.WriteLine("GameHUD: Quit button pressed!");

                    Globals.GameInstance.Exit(); // exit the game
                }
            }
        }
    }
}
