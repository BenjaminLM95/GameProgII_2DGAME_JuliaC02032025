#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using GameProgII_2DGame_Julia_C02032025.Components;

#endregion

/// <summary>
/// Manages the scripts, the instance, and holds globally accessible variables
/// </summary>
internal class Globals
{
    public static Globals _instance {  get; private set; }
    public static Globals Instance => _instance ??= new Globals();

    public float GlobalScaleFactor { get; set; } = 2.0f;  // Default scale factor of 2
    public GraphicsDevice GraphicsDevice { get; set; }

    public static ContentManager content;
    public static SpriteBatch spriteBatch;

    public static float TimeScale { get; set; } = 0f; // set game pause/unpause
    public static Game GameInstance { get; set; }

    public MapSystem _mapSystem { get;  set; }
    public Component _component;
    public HealthSystem _healthSystem;
    public Player _player;
    public Sprite _sprite;
    public GameObject _gameObject;
    public TileMap _tileMap;
    public Enemy _enemy;
    public RangedEnemy _rangedEnemy;
    public GhostEnemy _ghostEnemy;
    public TurnManager _turnManager;
    public GameHUD _gameHUD;
    public Inventory _inventory;
    public Items _items;

    public Scene _scene;
    public Globals()
    {
        _instance = this;
        _scene = new Scene();
        _gameObject = new GameObject();
        _turnManager = new TurnManager();
    }   
}