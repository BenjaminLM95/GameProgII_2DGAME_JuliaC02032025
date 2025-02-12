using GameProgII_2DGAME_JuliaC02032025.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing;
using Point = Microsoft.Xna.Framework.Point;

internal class Scene
{
    
    // ---------- VARIABLES ---------- //
    // List of GameObjects
    List<GameObject> _gameObjects = new List<GameObject>();

    // ---------- METHODS ---------- //    

    /// <summary>
    /// Add a gameobject to the scene.
    /// </summary>
    /// <param name="gameObject"></param>
    public void AddGameObject(GameObject gameObject)
    {
        _gameObjects.Add(gameObject);
    }

    /// <summary>
    /// Call Update on all GameObjects
    /// </summary>
    public void Update()
    {
        foreach (var gameObject in _gameObjects)
        {
            gameObject.Update();
        }
    }

    /// <summary>
    /// Call Draw on all GameObjects
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var gameObject in _gameObjects)
        {
            gameObject.Draw(spriteBatch);
        }
    }
}

public static class Globals // don't need?
{
    public static float Time { get; set; }
    public static ContentManager Content { get; set; }
    public static SpriteBatch SpriteBatch { get; set; }
    public static Point WindowSize { get; set; }

    public static void Update(GameTime gt)
    {
        Time = (float)gt.ElapsedGameTime.TotalSeconds;
    }
}
