using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolvePseudoku
{
    public abstract class Region
    {
        public List<Cell> cells = new List<Cell>();

        public abstract int CalculatePossibleNumbers();
        public abstract bool CheckDiscrepancies();
    }
}
