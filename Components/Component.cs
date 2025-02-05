using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Component // Abstract
{
    // Reference to owner (gameobject)
    private GameObject _gameObject;

    // OnStart() - branch off GameObject AddComponent()
    protected virtual void AddComponent()
    {
        // add this to the list of Components (GObj has the list)
    }
    // OnEnable()
    private void OnEnable()
    {

    }
    // OnDisable()
    private void OnDisable()
    {

    }
    // Enabled bool
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

    }
    // Initialize()
    public void Initialize()
    {

    }
}
