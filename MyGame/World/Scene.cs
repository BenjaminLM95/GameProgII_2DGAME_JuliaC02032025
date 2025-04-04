using GameProgII_2DGame_Julia_C02032025.Components;
using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using Point = Microsoft.Xna.Framework.Point;

internal class Scene
{
    public static Scene ActiveScene; // change ??
    // ---------- VARIABLES ---------- //
    public List<GameObject> _gameObjects = new List<GameObject>();

    // ---------- METHODS ---------- //    

    // Add a gameobject to the scene.
    public void AddGameObject(GameObject gameObject)
    {
        _gameObjects.Add(gameObject);
    }
    // Remove a gameobject from the scene.
    public void RemoveGameObject(GameObject gameObject)
    {
        foreach (var comp in gameObject._components)
            comp.OnDestroy();

        _gameObjects.Remove(gameObject);
    }

    // Call Update on all GameObjects
    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds * Globals.TimeScale;

        // Create a copy of the game objects list to safely iterate through (for when enemy GO get removed)
        var gameObjectsToUpdate = new List<GameObject>(_gameObjects);

        foreach (var gameObject in gameObjectsToUpdate)
        {
            gameObject.Update(deltaTime);
            //Debug.WriteLine($"Enemy Count: {Enemy._enemies.Count}");
        }
    }

    // Call Draw on all GameObjects
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var gameObject in _gameObjects)
        {
            foreach (var component in gameObject._components)
            {
                component.Draw(spriteBatch);
            }
        }
    }
}