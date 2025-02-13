using GameProgII_2DGAME_JuliaC02032025.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using static System.Formats.Asn1.AsnWriter;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace GameProgII_2DGAME_JuliaC02032025
{
    public class Game1 : Game
    {
        // ---------- REFERENCES ---------- //
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        GameManager _gameManager;
        private MapSystem mapSystem;

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
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Globals.Content = Content;
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.SpriteBatch = _spriteBatch;

            // ***** MAP ***** //
            // Create Map GameObject & MapSystem component
            GameObject mapObject = new GameObject();
            mapSystem = new MapSystem();
            TileMap tileMap = new TileMap();

            // Add components to Map GameObject        
            mapObject.AddComponent(mapSystem);
            mapObject.AddComponent(tileMap);

            mapSystem.Start();
            tileMap.LoadTextures(Content);
            tileMap.Initialize();

            // Add created GameObject to the scene
            GameManager.Instance._scene.AddGameObject(mapObject);

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
            GameManager.Instance._scene.AddGameObject(playerObject);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            // Call Scene.cs's Update method within this instance
            // (which updates all GameObjects)
            GameManager.Instance._scene.Update(gameTime);
 
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if (mapSystem != null)
            {
                mapSystem.Draw(_spriteBatch);
            }

            // Draw all GameObjects in the scene
            GameManager.Instance._scene.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

/* PROJECT
Implement a Map system that can:
✅  Generate random maps (minimum 10x15)
 !  Load predefined maps from files
 ?  Handle different tile types (at minimum: walkable, non-walkable, and exit tiles)

Basic gameplay features:
✅  Player movement using WASD/ arrow keys
 ✅  Collision detection with non-walkable tiles
 ?  Level transition when player reaches exit tile
*/

