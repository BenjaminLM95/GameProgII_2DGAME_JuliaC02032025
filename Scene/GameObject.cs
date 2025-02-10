using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = Microsoft.Xna.Framework.Vector2;

internal class GameObject 
{    
    Scene _scene; // Reference to scene

    // ---------- VARIABLES ---------- //
    // Position / Rotation
    Vector2 position = new Vector2(100, 100);
    private float rotation = 50f;
    // bool isActive
    public bool IsActive { get; set; }
    private bool hasComponent { get; set; }
    // List of components
    public List<Component> _components = new List<Component>();

    // ---------- METHODS ---------- //
    // AddComponent()
    protected virtual void AddComponent()
    {
        // reference/call corresponding Component method
        _components.Add(new Component()); // TEST if works
        IsActive = true;
    }
    // RemoveComponent()
    protected virtual void RemoveComponent()
    {
        if (_components != null)
            _components.RemoveAt(1); // may need to change to remove differently
    }
    // HasComponent()
    protected virtual void HasComponent() // check if the gameobject has a component
    {
        if (_components != null)
            hasComponent = true;
    }
    // GetComponent(of type)
    protected virtual Component GetComponent(Component componentType)
    {
        // get current component
        return componentType;
    }
    // Destroy()
    protected virtual void Destroy()
    {

    }
    // Update() - branch of Scene Update()
    protected virtual void Update(GameTime gameTime)
    {

    }
    // Draw() - branch of Scene Draw()
    protected virtual void Draw(GameTime gameTime)
    {
        // reference/call corresponding Component method
    }

    internal void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < _components.Count; i++)
        {
            _components[i].Draw(spriteBatch);
        }
    }
}
