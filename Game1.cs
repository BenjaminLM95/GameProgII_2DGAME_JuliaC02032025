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

            // ***** MAP ***** //
            // Create Map GameObject & MapSystem component
            GameObject mapObject = new GameObject();
            MapSystem mapSystem = new MapSystem();
            // Add components to Map GameObject        
            mapObject.AddComponent(mapSystem);
            // Add created GameObject to the scene
            Globals.Instance._scene.AddGameObject(mapObject);

            // ***** PLAYER ***** //
            GameObject playerObject = new GameObject();
            Player player = new Player();
            Sprite playerSprite = new Sprite();
            HealthSystem playerHealth = new HealthSystem();

            playerObject.AddComponent(player);
            playerObject.AddComponent(playerSprite);
            playerObject.AddComponent(playerHealth);
            playerSprite.LoadSprite("player");

            Globals.Instance._player = player;
            Globals.Instance._scene.AddGameObject(playerObject);

            // ***** ENEMY ***** //           
            Enemy enemyComponent = new Enemy();
            Globals.Instance._enemy = enemyComponent;
            // Spawn enemies for the first level
            enemyComponent.SpawnEnemies(5);

            // ***** COMBAT ***** //
            GameObject combatObj = new GameObject();
            Combat combat = Combat.Instance;
            combatObj.AddComponent(combat);

            Globals.Instance._combat = combat;
            Globals.Instance._scene.AddGameObject(combatObj);
            combat.Start();

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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Globals.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            // Draw all scene objects (including all GameObjects & thier Components)
            Globals.Instance._scene.Draw(Globals.spriteBatch);

            Globals.spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

