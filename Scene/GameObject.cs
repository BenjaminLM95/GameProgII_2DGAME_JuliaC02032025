using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

internal class GameObject 
{    
    Scene _scene; // Reference to scene

    // ---------- VARIABLES ---------- //
    // Position / Rotation
    Vector2 position = new Vector2(100, 100);
    private float rotation = 50f;
    // bool isActive
    public bool IsActive { get; set; }
    // List of components
    List<Component> _components = new List<Component>();

    // ---------- METHODS ---------- //
    // AddComponent()
    protected virtual void AddComponent()
    {
        // reference/call corresponding Component method
        IsActive = true;
    }
    // RemoveComponent()
    protected virtual void RemoveComponent()
    {

    }
    // HasComponent()
    protected virtual void HasComponent()
    {

    }
    // GetComponent(of type)
    protected virtual void GetComponent(Component componentType)
    {

    }
    // Destroy()
    protected virtual void Destroy()
    {

    }
    // Update() - branch of Scene Update()
    protected virtual void Update()
    {

    }
    // Draw() - branch of Scene Draw()
    protected virtual void Draw()
    {
        // reference/call corresponding Component method
    }
}
