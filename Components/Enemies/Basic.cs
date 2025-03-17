using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProgII_2DGAME_JuliaC02032025.Components.Enemies
{
    internal class Basic : Enemy
    {
        private Globals gameManager;

        private Texture2D basicEnemy;
        // Basic enemy behavior with Enemy base

        // ---------- METHODS ---------- //

        public override void Start()
        {
            gameManager = Globals.Instance;
            basicEnemy = Globals.content.Load<Texture2D>("BasicEnemy");
        }

        private void Draw()
        {
            
        }
        private void Move()
        {
            
        }

        public void Attack()
        {

        }
    }
}
