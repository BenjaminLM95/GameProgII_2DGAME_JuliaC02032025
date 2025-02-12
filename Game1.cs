using GameProgII_2DGAME_JuliaC02032025.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameProgII_2DGAME_JuliaC02032025.Components;

namespace GameProgII_2DGAME_JuliaC02032025
{
    public class Game1 : Game
    {
        // ---------- REFERENCES ---------- //
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Player _player;
        private Scene _scene;
        GameObject _gameObject;
        private MapSystem _mapSystem;

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
            _scene = new Scene();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create Player as a GameObject
            _player = new Player(Content);
            GameObject playerObject = new GameObject();
            playerObject.AddComponent(_player);

            // Load Map textures
            MapSystem map = new MapSystem(
            Content.Load<Texture2D>("floor"),
            Content.Load<Texture2D>("obstacle"),
            Content.Load<Texture2D>("start"),
            Content.Load<Texture2D>("exit")
            );

            // Add player to the scene
            _scene.AddGameObject(playerObject);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            _scene.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            //_scene.Draw(_spriteBatch); // draw all gameobjects !!!
            _mapSystem.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
/*
 * PROJECT
Implement a Map system that can:
    Generate random maps (minimum 10x15)
    Load predefined maps from files
    Handle different tile types (at minimum: walkable, non-walkable, and exit tiles)

Basic gameplay features:
    Player movement using WASD/ arrow keys
    Collision detection with non-walkable tiles
    Level transition when player reaches exit tile

!!! NOTE
    inheriting classes dont need to override unless ther are part of that 'group' of the class
    so getting Update(), sometimes would only need to call update of the branch class (like scene - component)
*/

