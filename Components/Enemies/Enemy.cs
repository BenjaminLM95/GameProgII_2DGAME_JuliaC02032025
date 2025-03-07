using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGAME_JuliaC02032025.Components.Enemies
{
    internal class Enemy : Component
    {
        private GameManager gameManager;
        private Basic basicEnemy;
        private Advanced advancedEnemy;

        // ---------- VARIABLES ---------- //
        private int minEnemyCount = 2;
        private int maxEnemyCount = 7;
        private List<Enemy> enemies = new List<Enemy>();

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

        private void PathfindToPlayer()
        {
            // based on the position of the player in Player.cs,
            // enemy finds closest possible path without obstacles to get to them.
            // ref: TileMap.cs, Player.cs
        }

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
