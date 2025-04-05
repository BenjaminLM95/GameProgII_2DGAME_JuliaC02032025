using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    /// <summary>
    /// Represents the map of tiles in the game, including managing tile textures and rendering the map.
    /// </summary>
    internal class TileMap : Component
    {
        Globals globals;

        // ---------- VARIABLES ---------- //    
        private SpriteBatch _spriteBatch;
        private List<Sprite> _tileSprites = new List<Sprite>();
        public int mapWidth { get; private set; } = 25;
        public int mapHeight { get; private set; } = 15;

        // Tile textures
        public Texture2D floorTexture { get; private set; }
        public Texture2D obstacleTexture { get; private set; }
        public Texture2D wallTexture { get; private set; }
        public Texture2D startTexture { get; private set; }
        public Texture2D exitTexture { get; private set; }
        public Texture2D enemyTexture { get; private set; }
        public Texture2D enemyGhostTexture { get; private set; }
        public Texture2D healthPotionTexture { get; private set; }
        public Texture2D fireScrollTexture { get; private set; }
        public Texture2D lightningScrollTexture { get; private set; }
        public Texture2D warpScrollTexture { get; private set; }
        public Texture2D playerTexture { get; private set; }
        public Texture2D turnIndicatorTexture { get; private set; }
        public Texture2D emptyInvTexture { get; private set; }
        public Texture2D playerFireballProj { get; private set; }
        public Texture2D archerProj { get; private set; }
        public Texture2D bossTexture { get; private set; }
        public Texture2D bossProj { get; private set; }

        public TileMap() { }
        public TileMap(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

        // ---------- METHODS ---------- //
        public void ClearTiles()
        {
            _tileSprites.Clear();
            Debug.WriteLine("Tilemap: cleared");
        }

        public void Initialize() // Initialize with floor tiles
        {
            if (_tileSprites.Count > 0) return;

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
            Debug.WriteLine($"Tilemap: initialized with {_tileSprites.Count} tiles.");
        }

        public void LoadTextures(ContentManager content)
        {
            try
            {
                floorTexture = content.Load<Texture2D>("floor");
                obstacleTexture = content.Load<Texture2D>("obstacle");
                wallTexture = content.Load<Texture2D>("wall");
                startTexture = content.Load<Texture2D>("start");
                exitTexture = content.Load<Texture2D>("exit");
                enemyGhostTexture = content.Load<Texture2D>("ghost");
                enemyTexture = content.Load<Texture2D>("enemy");
                playerTexture = content.Load<Texture2D>("player");
                healthPotionTexture = content.Load<Texture2D>("healthPotion");
                fireScrollTexture = content.Load<Texture2D>("fireScroll");
                lightningScrollTexture = content.Load<Texture2D>("lightningScroll");
                warpScrollTexture = content.Load<Texture2D>("warpScroll");
                turnIndicatorTexture = content.Load<Texture2D>("turnindicator");
                emptyInvTexture = content.Load<Texture2D>("emptyInvTexture");
                playerFireballProj = content.Load<Texture2D>("player_FireballProj");
                archerProj = content.Load<Texture2D>("archer_proj");
                bossTexture = content.Load<Texture2D>("boss");
                bossProj = content.Load<Texture2D>("boss_proj");

                Debug.WriteLine("TileMap: Textures loaded successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"TileMap: Error loading textures: {ex.Message}");
            }
        }

        // Returns the tile at a specific coordinate.
        public Sprite GetTileAt(int x, int y)
        {
            // Check if the coordinates are within bounds before accessing the tile
            if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
            {
                Debug.WriteLine($"TileMap: Error GetTileAt({x}, {y}) is out of bounds!");
                return null;
            }

            int index = y * mapWidth + x;

            // Ensure _tileSprites has enough elements before accessing
            if (index < 0 || index >= _tileSprites.Count)
            {
                Debug.WriteLine($"TileMap: Error GetTileAt({x}, {y}) is out of bounds!");
                return null;
            }
            return _tileSprites[index];
        }
        public Sprite GetTileNeighbor(int x, int y)
        {
            Sprite currentTile = GetTileAt(x, y);

            for (int checkY = -1; checkY < 1; checkY++)
            {
                for (int checkX = -1; checkX < 1; checkX++)
                {
                    if (checkY == 0 && checkX == 0) {
                        continue;
                    }
                    // it's a neighbor of the current tile, get the tilemap tile projSprite of all these
                }
            }
            return null;
        }

        // Draw all tiles to the screen.
        public override void Draw(SpriteBatch spriteBatch) // take out override?
        {
            foreach (var tileSprite in _tileSprites)
            {
                if (tileSprite.Texture == null)
                {
                    Debug.WriteLine($"Warning: Tile at {tileSprite.Position} has no projSprite!");
                    continue;
                }
                Globals.spriteBatch.Draw(tileSprite.Texture, tileSprite.Position, Color.White);
            }
            //Debug.WriteLine("Tilemap successfully drawn.");
        }
    }
}

