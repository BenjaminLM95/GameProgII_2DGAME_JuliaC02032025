using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGAME_JuliaC02032025.Components.Enemies
{
    internal class Enemy : Component
    {
        private Globals gameManager;
        private HealthSystem healthSystem;
        private Pathfinding pathfinding;

        private Basic basicEnemy;
        private Advanced advancedEnemy;

        // ---------- VARIABLES ---------- //
        private int minEnemyCount = 2;
        private int maxEnemyCount = 7;
        private List<Enemy> enemies = new List<Enemy>();

        private bool isStunned = false;
        public bool enemyMovedOntoPlayerTile { get; private set; }

        // spawn mechanic on random area of the map, maximum and minimum count
        // move towards player / pathfinding
        // recognize combat turn/take turn & wait

        // ---------- METHODS ---------- //
        private void SpawnEnemy()
        {
            // spawn anywhere from min to max enemycount on random floor tiles
            // once spawned, add to enemies list.
            // ref: TileMap.cs
        }

        public void MoveTowardsPlayer(Player player)
        {
            if (!isStunned)
            {
                // Move enemy to next tile
                Vector2 playerPosition = gameManager._player.GameObject.Position;
                Vector2 nextMove = pathfinding.GetNextMove(playerPosition);
            }
            // based on the position of the player in Player.cs,
            // enemy finds closest possible path without obstacles to get to them.
            // ref: TileMap.cs, Player.cs
        }

        public void TakeDamage(int damage)
        {
            gameManager._healthSystem.TakeDamage(damage);
            isStunned = true; // stunned after taking damage
        }

        public void RecoverFromStun()
        {
            isStunned = false;
        }

        public void EnemyTurn()
        {
            // If stunned, just recover and skip action
            if (isStunned)
            {
                RecoverFromStun();
            }
            else
            {
                // enemy AI behavior: Move, attack, etc.
                MoveTowardsPlayer(player);
            }
        }

        // -------------------------------------------------------------------//
        private void CheckForPlayer() // return Vector2 ?
        {
            // to be used in Combat.cs, if enemy is on player enemy takes turn
        }

        private void HandleBehavior()
        {
            basicEnemy.Attack();
            // based on the type of enemy, each has their own attacks and logic.
        }

        // enemy types need to be added to the same enemies list,
        // but handle different attack types or movement. Make separate lists for each?
        // maybe have AddEnemy() and AddBasicEnemy() separately,
        // but adding each idv type adds them to overall enemy list.
    }
}
