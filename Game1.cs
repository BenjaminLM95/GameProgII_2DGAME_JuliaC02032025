using GameProgII_2DGAME_JuliaC02032025.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
            _gameManager._player = new Player(_gameManager._sprite.Texture, new Vector2(100, 100));
            _gameManager._scene = new Scene();

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create Player as a GameObject
            
            GameObject playerObject = new GameObject();
            //playerObject.AddComponent(_gameManager._player); // gamemanager null ref exception

            Texture2D texture = Content.Load<Texture2D>("player");
            _gameManager._sprite = new Sprite(texture, Vector2.Zero);
            _gameManager._player = new Player(texture, Vector2.Zero);

            // Add player to the scene
            _gameManager._scene.AddGameObject(playerObject);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            _gameManager._scene.Update(gameTime);
            _gameManager._player.ReadInput();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            //_scene.Draw(_spriteBatch); // draw all gameobjects !!!
            //_mapSystem.Draw(_spriteBatch); // mapsystem is null !!!
            //_spriteBatch.Draw(Texture, new Rectangle(50, 50, 50, 50), Color.White);
            _spriteBatch.Draw(_gameManager._sprite.Texture, _gameManager._sprite.Position, Color.White);

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

