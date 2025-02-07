using GameProgII_2DGAME_JuliaC02032025.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GameProgII_2DGAME_JuliaC02032025
{
    public class Game1 : Game
    {
        // ---------- REFERENCES ---------- //
        Scene _scene;
        GameObject _gameObject;
        // make a while loop while player's health is above 0?

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

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

