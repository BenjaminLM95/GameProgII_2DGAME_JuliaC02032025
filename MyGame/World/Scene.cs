using GameProgII_2DGAME_JuliaC02032025.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Point = Microsoft.Xna.Framework.Point;

internal class Scene
{
    // ---------- VARIABLES ---------- //
    public List<GameObject> _gameObjects = new List<GameObject>();

    // ---------- METHODS ---------- //    

    // Add a gameobject to the scene.
    public void AddGameObject(GameObject gameObject)
    {
        _gameObjects.Add(gameObject);
    }

    // Call Update on all GameObjects
    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (var gameObject in _gameObjects)
        {
            gameObject.Update(deltaTime);
        }
    }

    // Call Draw on all GameObjects
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var gameObject in _gameObjects)
        {
            gameObject.Draw(spriteBatch);
        }
    }
}
