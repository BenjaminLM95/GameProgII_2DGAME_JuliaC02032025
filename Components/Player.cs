using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class Player : Component
    {
        /// <summary>
        /// Responsible for handling player input, and updating the player's Position with Draw()
        /// </summary>
  
        GameManager _gameManager;

        // ---------- VARIABLES ---------- //
        // Get/Use health properties from HealthSystem      
        private float speed = 300f;
        private Sprite sprite;
        
        // ---------- METHODS ---------- //
        public override void Start()
        {
            sprite = GameObject.GetComponent<Sprite>();
            if (sprite == null)
            {
                sprite = new Sprite();
                GameObject.AddComponent(sprite); // Add a sprite if missing
            }
        }

        /// <summary>
        /// Updates the player's state.
        /// </summary>
        public override void Update(float deltaTime)
        {
            Vector2 position = GameObject.Position;
            KeyboardState KeyboardState = Keyboard.GetState();

            if (KeyboardState.IsKeyDown(Keys.W)) position.Y -= speed * deltaTime; // UP
            if (KeyboardState.IsKeyDown(Keys.A)) position.X -= speed * deltaTime; // LEFT
            if (KeyboardState.IsKeyDown(Keys.S)) position.Y += speed * deltaTime; // DOWN
            if (KeyboardState.IsKeyDown(Keys.D)) position.X += speed * deltaTime; // RIGHT     
            
            GameObject.Position = position;
        }
    }
}