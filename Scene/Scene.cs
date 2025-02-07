using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Scene
{
    // ---------- VARIABLES ---------- //
    // List of GameObjects
    List<GameObject> _gameObjects = new List<GameObject>();

    // ---------- METHODS ---------- //
    // AddGameObject()
    public void AddGameObject()
    {
        // logic addds GameObjects to list _gameObjects
    }
    // Draw()
    protected virtual void Draw()
    {
        // reference/call corresponding GameObject method
    }
    // Update()
    protected virtual void Update()
    {
        // reference/call corresponding GameObject method
    }
}
