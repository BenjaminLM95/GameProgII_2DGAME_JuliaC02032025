using GameProgII_2DGAME_JuliaC02032025.Components;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

internal class GameManager
{
    public Collision _collision;
    public Component _component;
    public HealthSystem _healthSystem;
    public MapSystem _mapSystem;
    public Player _player;
    public Sprite _sprite;
    public GameObject _gameObject;
    public Scene _scene;
}
public static class Globals 
{
    public static float Time { get; set; }
    public static ContentManager Content { get; set; }
    public static SpriteBatch SpriteBatch { get; set; }
    public static Point WindowSize { get; set; }

    public static void Initialize(ContentManager content, SpriteBatch spriteBatch)
    {
        Content = content;
        SpriteBatch = spriteBatch;
    }
    public static void Update(GameTime gt)
    {
        Time = (float)gt.ElapsedGameTime.TotalSeconds;
    }
}