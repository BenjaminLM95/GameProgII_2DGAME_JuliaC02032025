using GameProgII_2DGAME_JuliaC02032025.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

internal class Component // Abstract
{
    // ---------- REFERENCES ---------- //   
    private GameObject _gameObject; // Reference to owner (gameobject)
    Player _player;

    // ---------- METHODS ---------- //
    // OnStart() - branch off GameObject AddComponent()
    protected virtual Component AddComponent(Component newComponent)
    {
        //_player = new Player();
        _gameObject._components.Add(new Player());
        Console.Write($"Added new Player to {_gameObject._components}"); // another way to display debug for MonoGame?
        // add this to the list of Components (GObj has the list)
        return newComponent;
    }

    // Update()
    public void Update(GameTime gameTime)
    {

    }
    // Draw() - branch of GameObject Draw()
    protected virtual void Draw(SpriteBatch spriteBatch)
    {
        //Sprite.LoadSprites(content);
        //Texture2D player = content.Load<Texture2D>("component");
    }
    // OnDestroy()
    public void OnDestroy()
    {
        // remove is from the list that GameObject has
    }
    // Initialize()
    public void Initialize()
    {
        // add to list of components in GameObject
    }

    // ---------------------------------------------------------------------------------------- //
    // OnEnable() - don't need ( can be bool that is switched when component is active in the scene)
    private void OnEnable()
    {

    }
    // OnDisable() - don't need
    private void OnDisable()
    {

    }
    // Enabled bool
}
