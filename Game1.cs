using GameProgII_2DGame_Julia_C02032025.Components;
using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace GameProgII_2DGame_Julia_C02032025
{
    /// <summary>
    /// this fixes "Missing XML comment" warning
    /// </summary>
    public class Game1 : Game
    {
        // ---------- REFERENCES ---------- //
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static Game1 instance;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";            
            IsMouseVisible = true;
            instance = this;
        }

        protected override void Initialize()
        {
            // setting window size
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            //_graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// Loads content for the game, including textures and initializing game objects.
        /// </summary>
        protected override void LoadContent()
        {
            Globals.Instance.GraphicsDevice = _graphics.GraphicsDevice;
            if (Globals.Instance.GraphicsDevice == null)
            {
                Debug.WriteLine("GAME1: GraphicsDevice is not initialized!");
            }
            else
            {
                Debug.WriteLine("GAME1: GraphicsDevice initialized successfully!");
            }
            Globals.spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.content = Content;

            AddMap();

            AddPlayer();

            AddEnemy();

            AddCombat();

            AddItem(ItemType.HealthPotion);
            AddItem(ItemType.FireScroll);
            AddItem(ItemType.LightningScroll);
            AddItem(ItemType.WarpScroll);

            // ***** HUD ***** //
            GameHUD hud = new GameHUD();
            hud.LoadContent();
            hud.DrawHUD("Your text here");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Globals.Instance._scene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Globals.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            // Draw all scene objects (including all GameObjects & thier Components)
            Globals.Instance._scene.Draw(Globals.spriteBatch);

            Globals.spriteBatch.End();
            base.Draw(gameTime);
        }

        #region adding gameobjects & components
        // ***** PLAYER ***** //
        void AddPlayer()
        {
            GameObject playerObject = new GameObject();
            Player player = new Player();
            Sprite playerSprite = new Sprite();
            HealthSystem playerHealth = new HealthSystem(
            maxHealth: 100,
            type: HealthSystem.EntityType.Player
            );

            playerObject.AddComponent(player);
            playerObject.AddComponent(playerSprite);
            playerObject.AddComponent(playerHealth);
            playerSprite.LoadSprite("player");

            Globals.Instance._player = player;
            Globals.Instance._scene.AddGameObject(playerObject);
        }

        // ***** MAP ***** //
        void AddMap()
        {
            // Create Map GameObject & MapSystem component
            GameObject mapObject = new GameObject();
            MapSystem mapSystem = new MapSystem();
            // Add components to Map GameObject        
            mapObject.AddComponent(mapSystem);
            // Add created GameObject to the scene
            Globals.Instance._scene.AddGameObject(mapObject);
        }

        // ***** ENEMY ***** //           
        void AddEnemy()
        {
            Enemy enemyComponent = new Enemy();
            Globals.Instance._enemy = enemyComponent;
            // Spawn enemies for the first level
            enemyComponent.SpawnEnemies(5);
        }

        // ***** COMBAT ***** //
        void AddCombat()
        {
            GameObject combatObj = new GameObject();
            Combat combat = Combat.Instance;
            combatObj.AddComponent(combat);

            Globals.Instance._combat = combat;
            Globals.Instance._scene.AddGameObject(combatObj);
            combat.Start();
        }

        // ***** ITEMS ***** //
        void AddItem(ItemType itemType)
        {
            GameObject itemObject = new GameObject();
            Items itemComponent = new Items();
            Sprite itemSprite = new Sprite();

            // Load sprite based on item type
            string spriteName = GetSpriteNameForItemType(itemType);
            itemSprite.LoadSprite(spriteName);

            itemObject.AddComponent(itemComponent);
            itemObject.AddComponent(itemSprite);

            // Optional: You could add a method to the Items component to initialize with a specific type
            itemComponent.InitializeItemType(itemType);

            Globals.Instance._scene.AddGameObject(itemObject);
        }

        private string GetSpriteNameForItemType(ItemType itemType)
        {
            return itemType switch
            {
                ItemType.HealthPotion => "healthPotion",
                ItemType.FireScroll => "fireScroll",
                ItemType.LightningScroll => "lightningScroll",
                ItemType.WarpScroll => "warpScroll",
                _ => throw new ArgumentException("Unknown item type")
            };
        }
        #endregion
    }
}

