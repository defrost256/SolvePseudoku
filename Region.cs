using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolvePseudoku
{
    /// <summary>
    /// Base class for the different kind of regions
    /// </summary>
    public abstract class Region
    {
        /// <summary>
        /// The cells this region contains 
        /// </summary>
        public List<Cell> cells = new List<Cell>();

        public abstract int CalculatePossibleNumbers();
        public abstract bool CheckDiscrepancies();
    }
}
