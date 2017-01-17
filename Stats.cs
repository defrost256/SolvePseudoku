using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolvePseudoku
{
    class Stats
    {
        static Stats instance = null;
        public static Stats Instance {
            get
            {
                if (instance == null)
                    instance = new Stats();
                return instance;
            }
        }

        public int cycles = 0, subCycles = 0;
        public int aliveNodes = 0, deadNodes = 0, solutions = 0;
        public int queueSize = 0;
        public int allDecisionStates = 0;
        public float avgFindPerSubcycle = 0;

    }
}
