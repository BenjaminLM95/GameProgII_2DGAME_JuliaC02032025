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
            // Initialize GameManager BEFORE using it
            //_gameManager = new GameManager();

            // Initialize Scene inside GameManager if it's null
            //if (_gameManager._scene == null)
            //    _gameManager._scene = new Scene();

            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Globals.Content = Content;
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create and add Player GameObject
            GameObject playerObject = new GameObject();
            Player player = new Player();
            Sprite playerSprite = new Sprite();

            //Player playerComponent = new Player(Globals.Content.Load<Texture2D>("player"), new Vector2(100, 100));
            playerObject.AddComponent(player);
            playerObject.AddComponent(playerSprite);

            playerSprite.LoadSprite("player");

            GameManager.Instance._scene.AddGameObject(playerObject);

            //Globals.Initialize(Content, _spriteBatch);
            //base.LoadContent();           
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            GameManager.Instance._scene.Update(gameTime);
            // TODO: Add your update logic here
            //_gameManager._scene.Update(gameTime);
            //_gameManager._player.ReadInput();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Draw all GameObjects, including the player
            GameManager.Instance._scene.Draw(_spriteBatch);

            //_scene.Draw(_spriteBatch); // draw all gameobjects !!!
            //_mapSystem.Draw(_spriteBatch); // mapsystem is null !!!
            //_spriteBatch.Draw(Texture, new Rectangle(50, 50, 50, 50), Color.White);
            //_spriteBatch.Draw(_gameManager._sprite.Texture, _gameManager._sprite.Position, Color.White);

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

