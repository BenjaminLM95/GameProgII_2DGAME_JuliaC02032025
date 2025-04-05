using GameProgII_2DGame_Julia_C02032025.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Vector2 = Microsoft.Xna.Framework.Vector2;

internal class GameObject 
{    
    // ---------- VARIABLES ---------- //
    public Vector2 Position { get; set; } = new Vector2(100, 100);
    private float rotation = 50f;
    private bool hasComponent { get; set; }
    public bool IsDestroyed { get; set; } = false;

    public List<Component> _components = new List<Component>();

    // ---------- METHODS ---------- //

    // Adds component to the list & initializes it.
    public void AddComponent(Component component)
    {
        component.GameObject = this;
        _components.Add(component);
        component.OnAddedToGameObject(); 
        component.Start();
    }

    // Removes a component from the list.
    public void RemoveComponent(Component component)
    {
        if (_components != null)
            _components.Remove(component); 
    }

    // Returns the first component of the given type, or null if not found.
    public T GetComponent<T>() where T : Component
    {
        foreach (var component in _components)
        {
            if(component is T tcomponent) {
                return tcomponent; }
        }
        return null;
    }

    // Checks if the GameObject has a component of a specific type.
    public bool HasComponent<T>() where T : Component
    {
        return GetComponent<T>() != null;
    }

    // Call Update on all components.
    public void Update(float deltaTime)
    {
        foreach (var component in _components)
        {
            component.Update(deltaTime); 
        }
    }

    // Call Draw on all components.
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var component in _components)
        {
            component.Draw(spriteBatch);
        }
    }
    public void LoadContent()
    {
        foreach (var component in _components)
        {
            component.LoadContent();
        }
    }

    // Returns a component from a GameObject
    public static T FindObjectOfType<T>() where T : Component
    {
        foreach (GameObject obj in Globals.Instance._scene._gameObjects)
        {
            T component = obj.GetComponent<T>();
            if (component != null)
            {
                return component; 
            }
        }
        return null;
    }

    // returns all components of a given type T across all GameObjects in the scene
    public static List<T> GetAllComponents<T>()
    {
        List<T> components = new List<T>();

        foreach (GameObject obj in Globals.Instance._scene._gameObjects)
        {
            foreach (var component in obj._components)
            {
                if (component is T t)
                    components.Add(t);
            }
        }

        return components;
    }
}
