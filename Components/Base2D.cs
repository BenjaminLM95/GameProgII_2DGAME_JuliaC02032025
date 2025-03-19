#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

#endregion

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    public class Base2D
    {
        public Vector2 position, dimensions;
        public Texture2D myTexture;

        public Base2D(string path, Vector2 pos, Vector2 dims)
        {
            position = pos;
            dimensions = dims;

            myTexture = Globals.content.Load<Texture2D>(path); // !!!ERROR: Microsoft.Xna.Framework.Content.ContentLoadException: 'The content file was not found.'

        }

        public virtual void Update()
        {

        }
        public virtual void Draw()
        {
            if (myTexture != null)
            {
                Globals.spriteBatch.Draw(myTexture,
                    new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y),
                    null, Color.White, 0.0f,
                    new Vector2(myTexture.Bounds.Width / 2, myTexture.Bounds.Height / 2),
                    new SpriteEffects(), 0);
            }
        }
    }
}
