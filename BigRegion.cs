using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolvePseudoku
{
    /// <summary>
    /// Defines a 20 cell region of the Pseudoku, in which every number from 0 to 9 can only appear twice
    /// </summary>
    class BigRegion : Region
    {

        static readonly int[] possibleBigregionNums = { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9 };

        /// <summary>
        /// Calculates the remaining possible numbers in the region, and updates the possible numbers of the cells in the region accordingly
        /// </summary>
        /// <returns>0 on success, otherwise error code (1 -> three cells in the region have the same number, 2 -> a cell in the region has no possible numbers after update)</returns>
        public override int CalculatePossibleNumbers()
        {
            List<int> possible = new List<int>(possibleBigregionNums);
            foreach (Cell c in cells)
            {
                if (c.HasNum)
                {
                    int i = possible.FindIndex(n => n == c.num);
                    if (i == -1)
                        return 1;
                    possible.RemoveAt(i);
                }
            }
            IEnumerable<int> possibleSet = possible.Distinct();
            foreach (Cell c in cells)
            {
                if (c.UpdatePossible(possibleSet) <= 0 && !c.HasNum)
                    return 2;
            }
            return 0;
        }

        /// <summary>
        /// Checks if there are three cells in the region with the same number
        /// </summary>
        /// <returns>True, if the region is valid</returns>
        public override bool CheckDiscrepancies()
        {
            List<int> possible = new List<int>(possibleBigregionNums);
            int i;
            foreach(Cell c in cells)
            {
                if (c.HasNum && (i = possible.FindIndex(n => n == c.num)) == -1)
                    return false;
            }
            return true;
        }
    }
}
