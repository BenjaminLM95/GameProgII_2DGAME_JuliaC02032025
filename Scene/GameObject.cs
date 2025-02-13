using GameProgII_2DGAME_JuliaC02032025.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Vector2 = Microsoft.Xna.Framework.Vector2;

internal class GameObject 
{    
    // ---------- VARIABLES ---------- //
    public Vector2 Position { get; set; } = new Vector2(100, 100);
    private float rotation = 50f;
    private bool hasComponent { get; set; }

    public List<Component> _components = new List<Component>();

    // ---------- METHODS ---------- //
    void Start()
    {
       
    }

    /// <summary>
    /// Adds component to the list & initializes it.
    /// </summary>
    /// <param name="component"></param>
    public void AddComponent(Component component)
    {
        component.GameObject = this;
        _components.Add(component); 
        component.Start();
    }

    /// <summary>
    /// Removes a component from the list.
    /// </summary>
    /// <param name="component"></param>
    public void RemoveComponent(Component component)
    {
        if (_components != null)
            _components.Remove(component); 
    }

    /// <summary>
    /// Returns the first component of the given type, or null if not found.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetComponent<T>() where T : Component
    {
        foreach (var component in _components)
        {
            if(component is T tcomponent) {
                return tcomponent; }
        }
        return null;
    }

    /// <summary>
    /// Checks if the GameObject has a component of a specific type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool HasComponent<T>() where T : Component
    {
        return GetComponent<T>() != null;
    }

    // Update all components
    public void Update(float deltaTime)
    {
        foreach (var component in _components)
        {
            component.Update(deltaTime); 
        }
    }
    // Draw all components
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var component in _components)
        {
            component.Draw(spriteBatch);
        }
    }
}
