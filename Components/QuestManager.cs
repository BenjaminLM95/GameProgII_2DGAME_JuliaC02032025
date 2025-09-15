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

        // ----------VARIABLES-------------//

        public List<Quest> Quests = new List<Quest>();

        private int numberQuests = 3; //Set how many quest there are
        public int enemyKills;
        public bool winTheGame; 

        public void setAllTheQuest()
        {
            // TODO: Set the quests
        }

        public override void Start()
        {            
            // null checks & component assignment
            globals = Globals.Instance; // globals

            createQuests();
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
                Quests[1].SetInstruction("Buy 1 item");
                Quests[2].SetInstruction("Defeat the boss"); 
            }
        }
    }

    
}
