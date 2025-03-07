#region Using
using GameProgII_2DGAME_JuliaC02032025.Components;

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

internal class World
{
    public Base2D player;

    public World() 
    {
        player = new Base2D("2D\\player", new Vector2(100,100), new Vector2(48,48));
    }

    public virtual void Update()
    {
        player.Update();
    }

    public virtual void Draw()
    {
        player.Draw();
    }
}
