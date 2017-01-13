using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolvePseudoku
{
    class BigRegion : Region
    {

        static readonly int[] possibleBigregionNums = { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9 };

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
