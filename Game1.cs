using GameProgII_2DGame_Julia_C02032025.Components;
using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Numerics;
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
            _graphics.PreferredBackBufferWidth = 1280; _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

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

            AddItems();

            AddPlayer();

            AddEnemy();
            AddRangedEnemy();
            AddGhostEnemy();

            AddCombat();

            // ***** HUD ***** //
            GameObject hudObj = new GameObject();
            GameHUD hud = new GameHUD();
            Sprite hudSprite = new Sprite();

            hudObj.AddComponent(hud);
            hudObj.AddComponent(hudSprite);
            hudSprite.LoadSprite("emptyInvTexture");

            Globals.Instance._gameHUD = hud;
            Globals.Instance._scene.AddGameObject(hudObj);
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
            
            Globals.Instance._combat.DrawTurnIndicator(); // draw turn indicator
            Globals.Instance._gameHUD.DrawInventoryHUD(); // draw inventory slots HUD
            Globals.Instance._gameHUD.DrawScreen();

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
            Inventory inventory = new Inventory();

            playerObject.AddComponent(player);
            playerObject.AddComponent(playerSprite);
            playerObject.AddComponent(playerHealth);
            playerObject.AddComponent(inventory);
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

        // ***** ENEMIES ***** //
        void AddRangedEnemy()
        {
            RangedEnemy rangedEnemyComponent = Globals.Instance._rangedEnemy;
            if (rangedEnemyComponent == null)
            {
                rangedEnemyComponent = new RangedEnemy();
                Globals.Instance._rangedEnemy = rangedEnemyComponent;
            }

            // spawn a specific number of enemies
            int level = 2; // amount of enemies
            int enemyCount = Math.Clamp(level, 2, 5);

            for (int i = 0; i < enemyCount; i++)
            {
                // create ranged enemy game object
                GameObject rangedEnemyObg = new GameObject();

                // create components
                RangedEnemy newEnemy = new RangedEnemy();
                Sprite enemySprite = new Sprite();
                HealthSystem enemyHealth = new HealthSystem(
                    maxHealth: 80,
                    type: HealthSystem.EntityType.Enemy
                );

                // add components to enemy game object
                rangedEnemyObg.AddComponent(newEnemy);
                rangedEnemyObg.AddComponent(enemySprite);
                rangedEnemyObg.AddComponent(enemyHealth);

                // load sprite
                enemySprite.LoadSprite("archer");

                // get a random spawn tile
                Vector2 randomTile = Globals.Instance._mapSystem.GetRandomEmptyTile();

                if (randomTile != new Vector2(-1, -1))
                {
                    rangedEnemyObg.Position = randomTile;
                    
                    TileMap tileMap = Globals.Instance._mapSystem.Tilemap;

                    Globals.Instance._scene.AddGameObject(rangedEnemyObg);
                }
            }
        }

        void AddEnemy()
        {
           Enemy enemyComponent = Globals.Instance._enemy;
           if (enemyComponent == null)
           {
               enemyComponent = new Enemy();
               Globals.Instance._enemy = enemyComponent;
           }

           // spawn a specific number of enemies
           int level = 5; // amount of enemies
           int enemyCount = Math.Clamp(level, 2, 10);

           for (int i = 0; i < enemyCount; i++)
           {
               //create enemy game object
               GameObject enemyObject = new GameObject();

               // create components
               Enemy newEnemy = new Enemy();
               Sprite enemySprite = new Sprite();
               Pathfinding enemyPathfinding = new Pathfinding();
               HealthSystem enemyHealth = new HealthSystem(
                   maxHealth: 50,
                   type: HealthSystem.EntityType.Enemy
               );

               // add components to enemy game object
               enemyObject.AddComponent(newEnemy);
               enemyObject.AddComponent(enemySprite);
               enemyObject.AddComponent(enemyPathfinding);
               enemyObject.AddComponent(enemyHealth);

               // load sprite
               enemySprite.LoadSprite("enemy");

               // get a random spawn tile
               Vector2 randomTile = Globals.Instance._mapSystem.GetRandomEmptyTile();

               if (randomTile != new Vector2(-1, -1))
               {
                   enemyObject.Position = randomTile;

                   // initialize pathfinding if tilemap exists
                   TileMap tileMap = Globals.Instance._mapSystem.Tilemap;
                   if (tileMap != null)
                   {
                       enemyPathfinding.InitializePathfinding(tileMap);
                       Debug.WriteLine($"Enemy: Spawned and initialized pathfinding at position - {randomTile}");
                   }
                   else
                   {
                       Debug.WriteLine("Enemy: CRITICAL - Cannot initialize pathfinding, TileMap is NULL");
                   }

                   // add to scene
                   Globals.Instance._scene.AddGameObject(enemyObject);
               }
           }
        }
        void AddGhostEnemy()
        {
            Enemy enemyComponent = Globals.Instance._enemy;
            if (enemyComponent == null)
            {
                enemyComponent = new Enemy();
                Globals.Instance._enemy = enemyComponent;
            }

            // spawn a specific number of enemies
            int level = 3; // amount of enemies
            int enemyCount = Math.Clamp(level, 2, 4);

            for (int i = 0; i < enemyCount; i++)
            {
                //create enemy game object
                GameObject enemyObject = new GameObject();

                // create components
                GhostEnemy newEnemy = new GhostEnemy(); 
                Sprite enemySprite = new Sprite();
                HealthSystem enemyHealth = new HealthSystem(30, HealthSystem.EntityType.Enemy);

                enemyObject.AddComponent(newEnemy);
                enemyObject.AddComponent(enemySprite);
                enemyObject.AddComponent(enemyHealth);
                enemySprite.LoadSprite("ghost");

                // get a random spawn tile
                Vector2 randomTile = Globals.Instance._mapSystem.GetRandomEmptyTile();
                Globals.Instance._scene.AddGameObject(enemyObject);
            }
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
        void AddItems()
        {
            // single item GameObject to handle all item
            GameObject itemsObject = new GameObject();
            Items itemsComponent = new Items();

            // Add the component to the game object
            itemsObject.AddComponent(itemsComponent);

            Globals.Instance._items = itemsComponent;

            // Add the items manager to the scene
            Globals.Instance._scene.AddGameObject(itemsObject);
            // Items component handles spawning items after it starts
            Debug.WriteLine("Game1: Items manager created and added to scene");
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

