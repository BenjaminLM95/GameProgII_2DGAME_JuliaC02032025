using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProgII_2DGAME_JuliaC02032025.Components
{
    internal class Combat : Component
    {
        private Globals gameManager;

        private bool isPlayerTurn = true;
        private bool isEnemyTurn = false;

        // handle player & enemy turn
        // visual indicator of who's turn it is
        // handle stunned state & options during turn (reference healthSystem & collision)
        // while loop while player's health > 0

        // ---------- METHODS ---------- //

        private void Update()
        {
            if (isPlayerTurn) {
                PlayerTurn();
            }
            else if (isEnemyTurn) {
                EnemyTurn();
            }

            TurnIndicator();
            // override component Update() ?
            // CheckTurn()
            // TurnIndicator()
            // TakeTurn()
            // StunEffect() - have at beginning for carry-over into next turn?
        }
        private void CheckTurn()
        {
            if (gameManager._player.playerMovedOntoEnemyTile)
            {
                isPlayerTurn = true;
                isEnemyTurn = false;
            }
            else if (gameManager._enemy.enemyMovedOntoPlayerTile)
            {
                isPlayerTurn = false;
                isEnemyTurn = true;
            }
            // based on who moved onto opponent's tile, turn is theirs first
            // (i.e. player moves onto enemy, player's turn is first)
            // add turn check to respective components or deal with it here?
        }

        private void TakeTurn()
        {
            // check whose turn it is with CheckTurn()
            // if enemy, EnemyTurn()
            // if player, PlayerTurn()
        }

        private void PlayerTurn()
        {
            isPlayerTurn = false;
            isEnemyTurn = true;
            // turn options: deal damage to enemy, move, recover from stun effect
            // get move from input in Player.cs
            // end of turn: calculate damage from enemy
        }
        private void EnemyTurn()
        {
            isEnemyTurn = false;
            isPlayerTurn = true;
            // turn options: deal damage to player, move, recover from stun effect
            // get move from enemy pathfinding in Enemy.cs
            // end of turn: calculate damage from player
        }

        private void StunEffect()
        {
            // once damaged, entity's next turn can only be to recover from stun. all movement is blocked
        }

        private void TurnIndicator()
        {
            // make into Draw? 
            // based on the active entity (who's turn it is), draw an arrow sprite above their head
        }
    }
}
