using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using GameProgII_2DGame_Julia_C02032025.Components.Enemies;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal class Combat : Component
    {
        private Globals gameManager;

        private bool isPlayerTurn = true;

        private const int TILE_SIZE = 32;

        private Texture2D turnIndicatorTexture;

        // ---------- METHODS ---------- //

        public override void Start()
        {
            gameManager = Globals.Instance;
            turnIndicatorTexture = Globals.content.Load<Texture2D>("turnIndicator");
        }

        private void Update()
        {
            if (isPlayerTurn)
            {
                PlayerTurn();
            }
            else
            {
                EnemyTurn();
            }

            TurnIndicator();
        }
        private void CheckTurn()
        {
            if (gameManager._player.playerMovedOntoEnemyTile)
            {
                isPlayerTurn = true;
            }
            else if (gameManager._enemy.enemyMovedOntoPlayerTile)
            {
                isPlayerTurn = false;
            }
        }

        public void PlayerTurn()
        {
            if (!isPlayerTurn) return; // Prevent turn actions if it's not the player's turn

            Player player = Globals.Instance._player;
            Enemy enemy = CheckForAdjacentEnemy(player);

            if (enemy != null)
            {
                player.Attack(enemy);
            }

            isPlayerTurn = false;
        }
        private void EnemyTurn()
        {
            Enemy enemy = Globals.Instance._enemy;
            Player player = CheckForAdjacentPlayer(enemy);

            if (player != null)
            {
                enemy.Attack(player);
            }

            isPlayerTurn = true;
        }

        /// <summary>
        /// checks if enemy is next to the player 
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private Enemy CheckForAdjacentEnemy(Player player)
        {
            Vector2 playerPos = player.GameObject.Position;
            Enemy enemy = Globals.Instance._enemy;
            Vector2 enemyPos = enemy.GameObject.Position;

            if (IsAdjacent(playerPos, enemyPos))
            {
                return enemy;
            }

            return null;
        }

        /// <summary>
        /// check if player is next to the enemy
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        private Player CheckForAdjacentPlayer(Enemy enemy)
        {
            Vector2 enemyPos = enemy.GameObject.Position;
            Player player = Globals.Instance._player;
            Vector2 playerPos = player.GameObject.Position;

            if (IsAdjacent(enemyPos, playerPos))
            {
                return player;
            }

            return null;
        }
        private bool IsAdjacent(Vector2 pos1, Vector2 pos2)
        {
            float dx = Math.Abs(pos1.X - pos2.X);
            float dy = Math.Abs(pos1.Y - pos2.Y);

            return dx == TILE_SIZE && dy == 0 || dy == TILE_SIZE && dx == 0;
        }

        private void StunEffect()
        {
            // once damaged, entity's next turn can only be to recover from stun. all movement is blocked
        }

        // Turn Indicator
        private void TurnIndicator()
        {
            Vector2 indicatorPos;

            if (isPlayerTurn)
            {
                indicatorPos = Globals.Instance._player.GameObject.Position;
            }
            else
            {
                indicatorPos = Globals.Instance._enemy.GameObject.Position;
            }

            DrawTurnIndicator(indicatorPos);
        }

        private void DrawTurnIndicator(Vector2 position)
        {
            Globals.spriteBatch.Draw(
                turnIndicatorTexture,
                new Vector2(position.X, position.Y - TILE_SIZE),
                Color.White
            );
        }
    }
}
