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

        GameManager _gameManager;

        public static Game1 instance;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            instance = this;
        }

        public bool isGameOver { get; private set; }
        public void GameOver()
        {
            if (isGameOver)
            {
                // freeze the game 
            }
        }
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Globals.Content = Content;
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // ***** PLAYER ***** //
            // Create Player GameObject & Player/Sprite components
            GameObject playerObject = new GameObject();
            Player player = new Player();
            Sprite playerSprite = new Sprite();
            Collision collision = new Collision();

            // Add player & sprite component to Player GameObject
            playerObject.AddComponent(player);
            playerObject.AddComponent(playerSprite);
            playerObject.AddComponent(collision);
            playerSprite.LoadSprite("player");

            // Add created GameObject to the scene
            GameManager.Instance._scene.AddGameObject(playerObject);    
            
            // ***** MAP ***** //
            // Create Map GameObject & MapSystem component
            GameObject mapObject = new GameObject();
            MapSystem mapSystem = new MapSystem();

            // Add MapSystem component to Map GameObject
            mapObject.AddComponent(mapSystem);

            // Add created GameObject to the scene
            GameManager.Instance._scene.AddGameObject(mapObject);
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

            // Draw all GameObjects in the scene
            GameManager.Instance._scene.Draw(_spriteBatch);
            //GameManager.Instance._mapSystem.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
/*
 * PROJECT
Implement a Map system that can:
 !  Generate random maps (minimum 10x15)
 !  Load predefined maps from files
 ?  Handle different tile types (at minimum: walkable, non-walkable, and exit tiles)

Basic gameplay features:
✅  Player movement using WASD/ arrow keys
 ?  Collision detection with non-walkable tiles
 ?  Level transition when player reaches exit tile

!!! NOTE
    inheriting classes dont need to override unless ther are part of that 'group' of the class
    so getting Update(), sometimes would only need to call update of the branch class (like scene - component)
*/

