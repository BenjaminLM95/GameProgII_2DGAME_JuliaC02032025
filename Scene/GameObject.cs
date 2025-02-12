using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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

    /// <summary>
    /// Adds component to the list
    /// </summary>
    /// <param name="component"></param>
    public virtual void AddComponent(Component component)
    {
        // reference/call corresponding Component method
        _components.Add(component); // TEST if works
        Console.WriteLine($"Added component: {component.GetType().Name}");
        IsActive = true;
    }

    /// <summary>
    /// Removes a component from the list.
    /// </summary>
    /// <param name="component"></param>
    public virtual void RemoveComponent(Component component)
    {
        if (_components != null)
            _components.Remove(component); 
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
    public void Update()
    {
        foreach (var component in _components)
        {
            component.Update();
        }
    }
   
    internal void Draw(SpriteBatch spriteBatch)
    {
        if (_components.Count == 0)
            return; // Avoid drawing empty objects

        foreach (var component in _components)
        {
            component.Draw(spriteBatch);
        }
    }
}
