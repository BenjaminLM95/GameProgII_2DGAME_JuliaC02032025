using GameProgII_2DGame_Julia_C02032025.Components.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
    internal class BossEnemy : Enemy, ITurnTaker
    {
        // ---------- VARIABLES ---------- //
        Texture2D atkIndicatorNothing, atkIndicatorMove, atkIndicatorCharge, atkIndicatorShoot;
        private List<Texture2D> predictedActions = new List<Texture2D>();
        private Texture2D predictedAction = null;
        private int TILE_SIZE = 32;

        private bool hasShot = false;
        public BossEnemy() : base(EnemyType.Boss) // Set boss type
        {
            config.SpriteName = "boss";
            config.MaxHealth = 150;
            config.Damage = 20;
            config.MovementSpeed = 1;
        }

        // ---------- METHODS ---------- //
        public override void Start()
        {
            base.Start();
            LoadTextures();
        }

        public void LoadTextures()
        {
            try
            {
                atkIndicatorNothing = Globals.content.Load<Texture2D>("atkIndicatorNothing");
                atkIndicatorMove = Globals.content.Load<Texture2D>("atkIndicatorMove");
                atkIndicatorCharge = Globals.content.Load<Texture2D>("atkIndicatorCharge");
                atkIndicatorShoot = Globals.content.Load<Texture2D>("atkIndicatorShoot");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"BossEnemy: Failed to load turn indicator texture - {e.Message}");
            }
        }

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
                Debug.WriteLine("BossEnemy: No valid player reference.");
                manager.EndTurn();
                return;
            }
            

            // need to implement a distance check, since if close enough will charge
            float distance = Vector2.Distance(GameObject.Position, player.GameObject.Position) / TILE_SIZE;
            Vector2 shootDirection;

            Random random = new Random();
            int decision = random.Next(3); // Random choice for turn action between 0, 1, 2
            // Predict the next turn actions
            PredictNextTurn();

            if (distance <= 3)
            {
                // Close enough to charge
                if (decision == 0)
                {
                    Debug.WriteLine("BossEnemy: Doing nothing.");
                }
                else
                {
                    Debug.WriteLine("BossEnemy: Charging toward player.");
                    Charge(player.GameObject.Position, player);
                }
            }
            else
            {
                if (decision == 0)
                {
                    Debug.WriteLine("BossEnemy: Doing nothing.");
                }
                else if (decision == 1 && HasLineOfSightToPlayer(player, out shootDirection))
                {
                    Debug.WriteLine("BossEnemy: Shooting at player.");
                    ShootProjectile(shootDirection, player);
                }
                else
                {
                    Debug.WriteLine("BossEnemy: Moving toward player.");
                    MoveTowardsPlayer(player);
                }
            }

            manager.EndTurn();
        }

        // ---------- COMBAT OPTIONS ---------- //
        // _______________________________________MOVE //
        public override void MoveTowardsPlayer(Player player, bool debug = false)
        {
            base.MoveTowardsPlayer(player, debug);
        }
        // _______________________________________ATTACK //
        public override void Attack(Player player) // can either SHOOT or CHARGE
        {
            Vector2 shootDirection = Vector2.Zero;
            Vector2 playerPos = player.GameObject.Position;
            
            Random random = new Random();
            var choseToShoot = random.Next(2) == 0; // 50% chance to shoot or charge

            if (choseToShoot)
            {
                Debug.WriteLine("BossEnemy: Line of sight to player found. Chose to shoot.");
                ShootProjectile(shootDirection, player);
                hasShot = true; // shoot once per turn
                return;
            }
            else
            {
                Charge(playerPos, player);
            }
            base.Attack(player);
        }
        // _______________________________________CHARGE //
        private void Charge(Vector2 targetPosition, Player player)
        {
            // need to check for collision with obstacles, & if coll with player player takes damage
            // move three tiles towards player
            Vector2 direction = targetPosition - GameObject.Position;
            direction.Normalize();

            for (int i = 0; i < 3; i++) // move up to 3 tiles
            {
                Vector2 nextPos = GameObject.Position + direction * TILE_SIZE;
                Point nextTile = new Point((int)(nextPos.X / TILE_SIZE), (int)(nextPos.Y / TILE_SIZE));

                if (!IsTileWalkable(nextTile))
                {
                    Debug.WriteLine($"BossEnemy: Charge stopped by wall at {nextTile}");
                    break;
                }

                GameObject.Position = nextPos;

                if (Vector2.Distance(GameObject.Position, player.GameObject.Position) < TILE_SIZE)
                {
                    Debug.WriteLine("BossEnemy: Crashed into player during charge!");
                    HealthSystem playerHealth = player.GameObject.GetComponent<HealthSystem>();
                    playerHealth.ModifyHealth(-config.Damage);
                    break;
                }
            }
        }
        // _______________________________________SHOOT //
        private bool HasLineOfSightToPlayer(Player player, out Vector2 direction)
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
                        Debug.WriteLine($"BossEnemy: Line of sight confirmed in direction {direction}");
                        return true;
                    }

                    // if the tile is not walkable, break (block vision)
                    if (!IsTileSeeThrough(tileToCheck))
                    {
                        Debug.WriteLine($"BossEnemy: Line of sight blocked in direction {dir} at {tileToCheck}");
                        break;
                    }

                    checkPos += dir; // move to next tile in direction
                }
            }

            direction = Vector2.Zero;
            Debug.WriteLine("BossEnemy: No line of sight in any direction.");
            return false;
        }

        private void ShootProjectile(Vector2 direction, Player player)
        {
            if (direction == Vector2.Zero)
            {
                Debug.WriteLine("BossEnemy: Invalid direction, aborting projectile.");
                return;
            }

            // creating a projectile gameobject for enemies
            GameObject projectileObject = new GameObject();
            Sprite enemyProjSprite = new Sprite();
            Projectile projectile = new Projectile(
                enemyProjSprite, // enemy projectile sprite
                direction,       // direction the proj is going
                200f,            // proj speed
                player,          // ref. to the player
                Globals.Instance._mapSystem.Tilemap, // ref. to the tilemap
                ProjectileSource.Enemy);             // projectile's source is an enemy

            projectileObject.AddComponent(projectile);
            projectileObject.AddComponent(enemyProjSprite);
            enemyProjSprite.LoadSprite("boss_proj");

            projectileObject.Position = GameObject.Position;

            Globals.Instance._scene.AddGameObject(projectileObject);
            Debug.WriteLine($"BossEnemy: Fired a projectile at the playerin direction {direction}");
        }

        // ---------- TURN PREDICTION ---------- //
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (predictedAction == null)
            {
                Debug.WriteLine("BossEnemy: No predicted action.");
                return;
            }

            // get the position of the boss
            Vector2 position = GameObject.Position;

            // draw the predicted action texture above the boss's head
            Vector2 texturePosition = new Vector2(
                position.X + (TILE_SIZE - predictedAction.Width) / 2,
                position.Y - predictedAction.Height - 5 // position it above the head
            );

            Globals.spriteBatch.Draw(
                predictedAction,
                texturePosition,
                Color.White
            );


            base.Draw(spriteBatch);
        }

        private void PredictNextTurn()
        {
            // retrieves the choice the boss will make on their next turn by checking if they are close enough to attack
            // and check if they have line of sight to shoot
            // if no to both, do nothing
            // if close enough to charge (3 tiles), MoveTowardsPlayer()
            // if has line of sight but not close enough to attack, ShootProjectile()
            // if 3 tiles or less away, Charge()
            // Clear previous predictions
            predictedAction = atkIndicatorNothing;

            float distance = Vector2.Distance(GameObject.Position, player.GameObject.Position) / TILE_SIZE;
            Vector2 shootDirection;

            // Action 1: Doing nothing
            if (distance > 3 && !HasLineOfSightToPlayer(player, out shootDirection))
            {
                predictedAction = atkIndicatorMove; // should check random number, nothing happens a lot but just as often as moving??
            }
            // Action 2: Moving towards the player
            else if (distance > 3 && !HasLineOfSightToPlayer(player, out shootDirection))
            {
                predictedAction = atkIndicatorMove;
            }
            // Action 3: Charging towards the player (if within 3 tiles)
            else if (distance <= 3)
            {
                predictedAction = atkIndicatorCharge;
            }
            // Action 4: Shooting at the player (if line of sight exists)
            if (HasLineOfSightToPlayer(player, out shootDirection))
            {
                predictedAction = atkIndicatorShoot;
            }
        }
    }
}
