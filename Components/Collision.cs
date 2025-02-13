using System;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class Collision : Component
    {
        GameManager _gameManager;

        // ---------- VARIABLES ---------- //
        // rectangle for hitbox?       

        // ---------- METHODS ---------- //

        public override void Start()
        {
            _gameManager = GameManager.Instance; // Initialize GameManager
            _gameManager._player = GameObject.GetComponent<Player>();
            _gameManager._mapSystem = GameManager.Instance._gameObject.GetComponent<MapSystem>();

            if (_gameManager._player == null)
                Console.WriteLine("Collision: Player component not found!");

            if (_gameManager._mapSystem == null)
                Console.WriteLine("Collision: MapSystem component not found!");
        }

        /// <summary>
        /// Checks the current tile, if it's null you can move.
        /// </summary>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        public bool CanMove(Vector2 newPosition)
        {
            Point tilePos = GetTileCoordinates(newPosition);
            if (!_gameManager._mapSystem.IsValidTile(tilePos)) return false;

            var tile = _gameManager._mapSystem.GetTileAt(tilePos);
            if (tile == null) return false;

            if (tile.Texture == _gameManager._mapSystem._obstacleTexture)
            {
                Console.WriteLine("Collision: Can't move to obstacle!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check if the player can move to a target position.
        /// If the target is an exit, load the new map.
        /// </summary>
        /// <param name="targetPosition"></param>
        public void OnMoveAtempt(Vector2 targetPosition)
        {
            Point targetTilePos = GetTileCoordinates(targetPosition);
            var tile = _gameManager._mapSystem.GetTileAt(targetTilePos);
            if (tile == null) return;

            if (tile.Texture == _gameManager._mapSystem._exitTexture)
            {
                Console.WriteLine("Collision: Reached exit, loading new map...");
                _gameManager._mapSystem.GenerateMap();
                _gameManager._player.GameObject.Position = _gameManager._mapSystem.GetStartTilePosition();
            }
        }

        private Point GetTileCoordinates(Vector2 position)
        {
            return new Point((int)(position.X / 16), (int)(position.Y / 16));
        }
    }
}
