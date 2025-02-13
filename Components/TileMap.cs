using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class TileMap : Component
    {
        GameManager _gameManager;

        // ---------- VARIABLES ---------- //    
        private SpriteBatch _spriteBatch;
        private List<Sprite> _tileSprites = new List<Sprite>();
        public int mapWidth { get; private set; } = 25;
        public int mapHeight { get; private set; } = 15;

        // Tile textures
        public Texture2D floorTexture { get; private set; }
        public Texture2D obstacleTexture { get; private set; }
        public Texture2D startTexture { get; private set; }
        public Texture2D exitTexture { get; private set; }

        public TileMap() { }
        public TileMap(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

        // ---------- METHODS ---------- //
        
        public void Initialize() // Initialize with floor tiles
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    Sprite tileSprite = new Sprite
                    {
                        Position = new Vector2(x * 32, y * 32), // Set position based on tile size
                        Texture = floorTexture
                    };
                    _tileSprites.Add(tileSprite);
                }
            }
        }

        public void LoadTextures(ContentManager content)
        {
            floorTexture = content.Load<Texture2D>("floor");
            obstacleTexture = content.Load<Texture2D>("obstacle");
            startTexture = content.Load<Texture2D>("start");
            exitTexture = content.Load<Texture2D>("exit");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var tileSprite in _tileSprites)
            {
                spriteBatch.Draw(tileSprite.Texture, tileSprite.Position, Color.White);
            }
        }

        // Get the tile at a specific position
        public Sprite GetTileAt(int x, int y)
        {
            // Check if the coordinates are within bounds before accessing the tile
            if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
            {
                Debug.WriteLine("TileMap: Coordinates out of bounds!");
                return null; // Return null if out of bounds
            }

            return _tileSprites[y * mapWidth + x];
        }
    }
}

