using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    internal class QuestManager : Component
    {
        // ---------- REFERENCES ---------- //
        Globals globals;
        Player player; 

        // ----------VARIABLES-------------//

        public List<Quest> Quests = new List<Quest>();

        private int numberQuests = 3; //Set how many quest there are
        public int enemyKills; // This tells how many enemies the player has kill. Player should have a variable for this.
        //public bool completeAllQuests; // This will be true if you complete all the quests
        

        public void setAllTheQuest()
        {
            // TODO: Set the quests
        }

        public override void Start()
        {            
            // null checks & component assignment
            globals = Globals.Instance; // globals

            // Gets the player
            player = GameObject.FindObjectOfType<Player>();

            // Obtain the number of kills from the player
            enemyKills = globals._player.numKills; 
            createQuests();
        }

        public override void Update(float deltaTime)
        {
            if(player == null)
                player = GameObject.GetComponent<Player>();

            if (enemyKills != globals._player.numKills) 
            {
                enemyKills = globals._player.numKills;
                if (enemyKills >= 3 && !Quests[0].completed) 
                {
                    Quests[0].completed = true;
                }
            }

            if (!Quests[1].completed) 
            {
                if (globals._player.numPurchases > 1) 
                {
                    Quests[1].completed = true;
                }
            }

            if (!Quests[2].completed) 
            {
                if(globals._player.killBoss)
                    Quests[2].completed = true;
            }

            if (!player.completeAllQuests) 
            {
                if (Quests[0].completed && Quests[1].completed && Quests[2].completed) 
                {
                    player.completeAllQuests = true;
                }
            }

        }

        public void createQuests() 
        {
            for(int i = 0; i < numberQuests; i++) 
            {
                Quests.Add(new Quest()); 
            }

            if(Quests.Count >= 3) 
            {
                Quests[0].SetInstruction("Kill 3 enemies");
                Quests[1].SetInstruction("Buy 2 items");
                Quests[2].SetInstruction("Defeat the boss"); 
            }
        }

        public void resetQuests() 
        {
            for (int i = 0; i < Quests.Count; i++) 
            {
                Quests[i].completed = false; 
            }
        }
    }

    
}
