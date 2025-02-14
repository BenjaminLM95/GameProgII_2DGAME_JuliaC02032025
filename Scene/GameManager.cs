using GameProgII_2DGAME_JuliaC02032025.Components;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

/// <summary>
/// Manages the game state, including handling the scene and game objects.
/// </summary>
internal class GameManager
{
    public static GameManager _instance {  get; private set; }
    public static GameManager Instance => _instance ??= new GameManager();

    public Component _component;
    public HealthSystem _healthSystem;
    public Player _player;
    public Sprite _sprite;
    public GameObject _gameObject;
    public TileMap _tileMap;

    public Scene _scene;
    public GameManager()
    {
        _instance = this;
        _scene = new Scene();
        _gameObject = new GameObject();
    }

    public MapSystem _mapSystem { get;  set; }
}

// Global utility class to store game-related properties 
public static class Globals 
{
    public static ContentManager Content { get; set; }
    public static SpriteBatch SpriteBatch { get; set; }
}