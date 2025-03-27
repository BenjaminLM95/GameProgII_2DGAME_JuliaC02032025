using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal class GameHUD : Component
    {
        Globals globals;

        SpriteFont myFont;
        Vector2 fontPos;
        private SpriteBatch spriteBatch;

        public override void Start()
        {
            if (Globals.Instance.GraphicsDevice == null)
            {
                Debug.WriteLine("GraphicsDevice is not initialized yet!");
            }
        }
        // Loads the SpriteFont asset
        public override void LoadContent()
        {
            if (Globals.Instance.GraphicsDevice != null)
            {
                spriteBatch = new SpriteBatch(Globals.Instance.GraphicsDevice);
                myFont = Globals.content.Load<SpriteFont>("MyFont");

                Viewport viewport = Globals.Instance.GraphicsDevice.Viewport;
                fontPos = new Vector2(viewport.Width / 2, viewport.Height / 2);
                Debug.WriteLine("GameHUD: My font loaded");
            }
            else
            {
                Debug.WriteLine("GameHUD: Cannot initialize SpriteBatch, GraphicsDevice is still null.");
            }
        }
        public override void Update(float deltaTime)
        {
            // HUD elements here
        }
        // Draws spritefont
        public void DrawHUD(string text)
        {
            if (spriteBatch != null)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                Debug.WriteLine($"Drawing text at position: {fontPos}");
                spriteBatch.DrawString(myFont, text, fontPos, Color.White);
                Debug.WriteLine("GameHUD: My font drawn");
                spriteBatch.End();
            }
        }
    }
}
// show player/enemy health
// show itemPool
// show turn indicator