using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace GameProgII_2DGame_Julia_C02032025.Components.Enemies
{
    internal class RangedEnemy : Enemy
    {
        private bool hasShot = false;
        const int MIN_DISTANCE = 3; // minimum tiles to keep away from player

        public RangedEnemy() : base(EnemyType.Archer) // Set default type
        {
            config.SpriteName = "archer"; // Use archer sprite
            config.MaxHealth = 40;
            config.Damage = 10;
            config.MovementSpeed = 1; // Archers move if no line of sight
           
        }

        // ---------- METHODS ---------- //
        public override void StartTurn(TurnManager manager)
        {
            if (IsDead())
            {
                manager.EndTurn();
                return;
            }

            hasMoved = false;
            hasTakenTurn = false;

            if (player == null || player.GameObject == null)
            {
                //Debug.WriteLine("RangedEnemy: No valid player reference.");
                manager.EndTurn();
                return;
            }


            Vector2 shootDirection;
            if (HasLineOfSightToPlayer(player, out shootDirection))
            {
                //Debug.WriteLine("RangedEnemy: Attacking player from distance!");
                ShootProjectile(shootDirection, player);
                hasShot = true;
            }
            else
            {
                //Debug.WriteLine("RangedEnemy: No line of sight, moving toward player...");
                MoveTowardsPlayer(player, debug: true);
            }

            manager.EndTurn();
        }

        public override void Update(float deltaTime)
        {
            Player player = GameObject.FindObjectOfType<Player>();
            if (player == null)
            {
                //Debug.WriteLine("RangedEnemy: Player not found.");
                return;
            }
        }

        public override void Attack(Player player)
        {
            Vector2 shootDirection;
            if (HasLineOfSightToPlayer(player, out shootDirection))
            {
                //Debug.WriteLine("RangedEnemy: Line of sight to player found. Shooting.");
                ShootProjectile(shootDirection, player);
                hasShot = true;
                return;
            }
        }
        // ---------- Shooting Logic ---------- //
        private bool HasLineOfSightToPlayer(Player player, out Vector2 direction, bool debug = false)
        {
            Vector2[] directions = new Vector2[] // can choose UP, DOWN, LEFT, RIGHT each turn for shoot direction
            {
                Vector2.UnitX,
                -Vector2.UnitX,
                Vector2.UnitY,
                -Vector2.UnitY
            };

            Vector2 enemyPos = GameObject.Position;
            Vector2 playerPos = player.GameObject.Position;

            foreach (Vector2 dir in directions)
            {
                Vector2 checkPos = enemyPos + dir;

                while (true)
                {
                    Point tileToCheck = new Point((int)(checkPos.X / 32), (int)(checkPos.Y / 32));

                    // if we reached the player's tile
                    if (Vector2.DistanceSquared(checkPos, playerPos) < 1f)
                    {
                        direction = dir;
                        if (debug) Debug.WriteLine($"RangedEnemy: Line of sight confirmed in direction {direction}");
                        return true;
                    }

                    // if the tile is not walkable, break (block vision)
                    if (!IsTileSeeThrough(tileToCheck))
                    {
                        if (debug) Debug.WriteLine($"RangedEnemy: Line of sight blocked in direction {dir} at {tileToCheck}");
                        break;
                    }
                    checkPos += dir; // move to next tile in direction
                }
            }
            direction = Vector2.Zero;
            if (debug) Debug.WriteLine("RangedEnemy: No line of sight in any direction.");
            return false;
        }
        
        private void ShootProjectile(Vector2 direction, Player player)
        {
            if (direction == Vector2.Zero) {
                Debug.WriteLine("RangedEnemy: Invalid direction, aborting projectile.");
                return;
            } 

            // creating a projectile gameobject for enemies
            GameObject projectileObject = new GameObject();
            Sprite enemyProjSprite = new Sprite();
            Projectile projectile = new Projectile(
                enemyProjSprite, // enemy projectile sprite
                direction,       // direction the proj is going
                150f,            // proj speed
                player,          // ref. to the player
                Globals.Instance._mapSystem.Tilemap, // ref. to the tilemap
                ProjectileSource.Enemy);             // projectile's source is an enemy

            projectileObject.AddComponent(projectile);
            projectileObject.AddComponent(enemyProjSprite);
            enemyProjSprite.LoadSprite("archer_proj");

            projectileObject.Position = GameObject.Position;

            Globals.Instance._scene.AddGameObject(projectileObject);
            Debug.WriteLine($"RangedEnemy: Fired a projectile at the playerin direction {direction}");
        }
        // ---------- Movement ---------- //
        public override void MoveTowardsPlayer(Player player, bool debug = false)
        {
            Vector2 enemyPos = GameObject.Position;
            Vector2 playerPos = player.GameObject.Position;

            int tilesX = Math.Abs((int)(playerPos.X / 32) - (int)(enemyPos.X / 32));
            int tilesY = Math.Abs((int)(playerPos.Y / 32) - (int)(enemyPos.Y / 32));
            int tileDistance = tilesX + tilesY;

            if (debug) Debug.WriteLine($"RangedEnemy: Distance to player is {tileDistance} tiles");

            // if already too close, move away from player
            if (tileDistance < MIN_DISTANCE)
            {
                if (debug) Debug.WriteLine("RangedEnemy: Too close to player, moving away");

                Vector2 awayDirection = enemyPos - playerPos;

                // normalize pos to get a unit vector
                if (awayDirection != Vector2.Zero)
                {
                    awayDirection.Normalize();
                }
                else {
                    // default direction if at the same position
                    awayDirection = new Vector2(1, 0);
                }

                TryMoveInDirection(awayDirection, debug);
            }
            // if at a good distance or further, use regular movement logic
            // but stop if too close to the player
            else
            {
                if (debug) Debug.WriteLine($"RangedEnemy: At good distance ({tileDistance} tiles), using regular movement");

                // before moving, check if the movement would bring us too close
                Vector2 dirToPlayer = playerPos - enemyPos;
                if (dirToPlayer != Vector2.Zero)
                {
                    dirToPlayer.Normalize();
                }

                // calculate potential new position
                Vector2 potentialNewPos = enemyPos + dirToPlayer * config.MovementSpeed * 32;

                // calculate new distance if we were to move
                int newTilesX = Math.Abs((int)(playerPos.X / 32) - (int)(potentialNewPos.X / 32));
                int newTilesY = Math.Abs((int)(playerPos.Y / 32) - (int)(potentialNewPos.Y / 32));
                int newTileDistance = newTilesX + newTilesY;

                // only move if it wouldn't be too close
                if (newTileDistance >= MIN_DISTANCE)
                {
                    base.MoveTowardsPlayer(player, debug);
                }
                if (debug) Debug.WriteLine("RangedEnemy: Movement would bring too close to player, staying put");
            }
        }

        private void TryMoveInDirection(Vector2 direction, bool debug = false)
        {
            // try to move in the specified direction
            Vector2 moveDirection = new Vector2(
                Math.Sign(direction.X),
                Math.Sign(direction.Y)
            );

            // try the main direction
            if (moveDirection.X != 0 && moveDirection.Y != 0)
            {
                if (TryMove(new Vector2(moveDirection.X, 0), debug)) // X first
                    return;
                
                if (TryMove(new Vector2(0, moveDirection.Y), debug)) // try Y
                    return;
            }
            else {
                if (TryMove(moveDirection, debug)) // for cardinal directions
                    return;
            }

            if (moveDirection.X != 0) // vertically
            {
                if (TryMove(new Vector2(0, 1), debug) || TryMove(new Vector2(0, -1), debug))
                    return;
            }
            else if (moveDirection.Y != 0) // horizontally
            {
                if (TryMove(new Vector2(1, 0), debug) || TryMove(new Vector2(-1, 0), debug))
                    return;
            }
            if (debug)
                Debug.WriteLine("RangedEnemy: Unable to move in any direction");
        }

        // try a specific move
        private bool TryMove(Vector2 direction, bool debug = false)
        {
            Vector2 newPosition = GameObject.Position + direction * config.MovementSpeed * 32;
            Point newTile = new Point((int)(newPosition.X / 32), (int)(newPosition.Y / 32));

            if (IsTileWalkable(newTile))
            {
                GameObject.Position = newPosition;
                if (debug)
                    Debug.WriteLine($"RangedEnemy: Moved to {newPosition}");

                return true;
            }
            return false;
        }
    }
}