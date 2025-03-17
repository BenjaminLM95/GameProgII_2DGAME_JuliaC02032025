using GameProgII_2DGAME_JuliaC02032025.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Formats.Asn1.AsnWriter;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace GameProgII_2DGAME_JuliaC02032025
{
    public class Game1 : Game
    {
        // ---------- REFERENCES ---------- //
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private MapSystem mapSystem;

        public static Game1 instance;

        World world;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            instance = this;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Loads content for the game, including textures and initializing game objects.
        /// </summary>
        protected override void LoadContent()
        {
            Globals.content = this.Content;
            Globals.spriteBatch = new SpriteBatch(GraphicsDevice);

            world = new World();

            // ***** MAP ***** //
            // Create Map GameObject & MapSystem component
            GameObject mapObject = new GameObject();
            MapSystem mapSystem = new MapSystem();
            TileMap tileMap = new TileMap();

            // Add components to Map GameObject        
            mapObject.AddComponent(mapSystem);
            mapObject.AddComponent(tileMap);

            mapSystem.Start();
            tileMap.LoadTextures(Content);
            tileMap.Initialize();

            // Add created GameObject to the scene
            Globals.Instance._scene.AddGameObject(mapObject);

            // ***** PLAYER ***** //
            // Create Player GameObject & Player/Sprite components
            GameObject playerObject = new GameObject();
            Player player = new Player();
            Sprite playerSprite = new Sprite();

            // Add player & sprite component to Player GameObject
            playerObject.AddComponent(player);
            playerObject.AddComponent(playerSprite);
            playerSprite.LoadSprite("player");

            // Add created GameObject to the scene
            Globals.Instance._scene.AddGameObject(playerObject);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            world.Update();
            // Call Scene.cs's Update method within this instance
            // (which updates all GameObjects)
            //Globals.Instance._scene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Globals.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            //if (mapSystem != null)
            //{
            //    mapSystem.Draw(_spriteBatch);
            //}

            // Draw all GameObjects in the scene
            //Globals.Instance._scene.Draw(_spriteBatch);

            world.Draw();

            Globals.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

