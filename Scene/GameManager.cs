using GameProgII_2DGAME_JuliaC02032025.Components;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

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

    public void Start()
    {
        _mapSystem = GameObject.FindObjectOfType<MapSystem>();       
    }
}
public static class Globals 
{
    public static float Time { get; set; }
    public static ContentManager Content { get; set; }
    public static SpriteBatch SpriteBatch { get; set; }
    public static Point WindowSize { get; set; }
}