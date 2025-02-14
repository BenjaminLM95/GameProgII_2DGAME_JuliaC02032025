using Microsoft.Xna.Framework.Graphics;

internal class Component // Abstract
{
    GameManager _gameManager;

    // The GameObject this component is attached to.
    public GameObject GameObject { get; set; }

    // ---------- METHODS ---------- //

    public virtual void Start() { }

    public virtual void Update(float deltaTime) { }

    public virtual void Draw(SpriteBatch spriteBatch) { }
}
