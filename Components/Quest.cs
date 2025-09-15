using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProgII_2DGame_Julia_C02032025.Components
{
    public enum progression 
    {
        Unnasigned,
        InProgress,
        Completed
    }

    internal class Quest
    {
        public bool assigned;
        public bool completed;
        public progression questProgression;
        public string instructions; 

        public Quest() 
        {
            assigned = false;
            completed = false;
            questProgression = progression.Unnasigned;
        }


        public void SetInstruction(string textInstruction)
        {
            instructions = textInstruction;
        }
    }
}
