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
        Texture2D atkIndicatorNothing, atkIndicatorMove, atkIndicatorCharge, atkIndicatorShoot;
        private int TILE_SIZE = 32;

        private bool hasShot = false;
        public BossEnemy() : base(EnemyType.Boss) // Set boss type
        {
            config.SpriteName = "boss";
            config.MaxHealth = 100;
            config.Damage = 20;
            config.MovementSpeed = 1;
        }

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
                Debug.WriteLine($"TurnManager: Failed to load turn indicator texture - {e.Message}");
            }
        }

        public override void MoveTowardsPlayer(Player player, bool debug = false)
        {
            base.MoveTowardsPlayer(player, debug);
        }

        public override void Attack(Player player)
        {
            Vector2 shootDirection = Vector2.Zero;
            Vector2 playerPos = player.GameObject.Position;
            // choose between charge / shootprojectile
            Random random = new Random();
            var choseToShoot = random.Next(2) == 0; // 50% chance to shoot or charge

            if (choseToShoot)
            {
                Debug.WriteLine("RangedEnemy: Line of sight to player found. Chose to shoot.");
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

        private void Charge(Vector2 direction, Player player)
        {
            // need to check for collision with obstacles, & if coll with player player takes damage
            base.Attack(player);
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
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (atkIndicatorNothing == null)
            {
                Debug.WriteLine("BossEnemy: texture2D is NULL!");
                return;
            }
            // read the boss's next movement choice, draw the corresponding indicator texture
            // get position from the current turn taker
            Vector2 position = GameObject.Position;

            Globals.spriteBatch.Draw(
                           atkIndicatorNothing,
                           new Vector2(
                               position.X + (TILE_SIZE - atkIndicatorNothing.Width) / 2,
                               position.Y - atkIndicatorNothing.Height - 5
                           ),
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
        }
        /*
         * Final Boss Requirements:
        More health than regular enemies.
        Handles their turn differently than regular enemies.
        Above their head they display an icon corresponding to what they will do on their next turn between these options:
            Nothing.
            Move towards the player.
            Shoot a projectile in the player's direction.
            Charge 3 tiles in the player's direction (Stopping if encounters the player or a wall, damaging the player if they encounter the player.
        If they are not aligned with the player when doing the charge or projectile attack, pick the closest direction they can shoot towards the player.
        Victory Condition:
            Clear game completion state when boss is defeated
            Option to restart game after victory
         */
    }
}
