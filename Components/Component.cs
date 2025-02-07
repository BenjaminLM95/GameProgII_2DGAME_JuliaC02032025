using GameProgII_2DGAME_JuliaC02032025.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Component // Abstract
{
    // ---------- REFERENCES ---------- //   
    private GameObject _gameObject; // Reference to owner (gameobject)
    Player _player;

    // ---------- METHODS ---------- //
    // OnStart() - branch off GameObject AddComponent()
    protected virtual void AddComponent()
    {
        // add this to the list of Components (GObj has the list)
        _player = new Player();
    }
    
    // Update()
    public void Update()
    {

    }
    // Draw() - branch of GameObject Draw()
    protected virtual void Draw()
    {

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
