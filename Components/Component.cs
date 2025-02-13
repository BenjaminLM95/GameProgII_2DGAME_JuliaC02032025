using Microsoft.Xna.Framework.Graphics;

internal class Component // Abstract
{
    GameManager _gameManager;

    /// <summary>
    /// The GameObject this component is attached to.
    /// </summary>
    public GameObject GameObject { get; set; }

    // ---------- METHODS ---------- //

    /// <summary>
    /// Called when the component is added to a GameObject.
    /// </summary>
    public virtual void Start() { }

    /// <summary>
    /// Updates the component logic every frame.
    /// </summary>
    public virtual void Update(float deltaTime) { }

    /// <summary>
    /// Draws the component if needed.
    /// </summary>
    public virtual void Draw(SpriteBatch spriteBatch) { }

    /// <summary>
    /// Called when the component is removed or the GameObject is destroyed.
    /// </summary>
    public virtual void OnDestroy() { }
}
