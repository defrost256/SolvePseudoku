using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolvePseudoku
{
    /// <summary>
    /// Defines a 2 cell region of the pseudoku containing a 2 digit prime number when read left-right or top-bottom
    /// </summary>
    class PrimeRegion : Region
    {

        static Dictionary<int, int[]> primeTensToOnes = null, primeOnesToTens = null;
        /// <summary>
        /// Dictionary mapping the ones of all possible two digit prime number to their tens
        /// </summary>
        public static Dictionary<int, int[]> TensToOnes
        {
            get
            {
                if (primeTensToOnes == null)
                {
                    primeTensToOnes = new Dictionary<int, int[]>();
                    primeTensToOnes.Add(0, new int[] { 2, 3, 5, 7 });
                    primeTensToOnes.Add(1, new int[] { 1, 3, 7, 9 });
                    primeTensToOnes.Add(2, new int[] { 3, 9 });
                    primeTensToOnes.Add(3, new int[] { 1, 7 });
                    primeTensToOnes.Add(4, new int[] { 1, 3, 7 });
                    primeTensToOnes.Add(5, new int[] { 3, 9 });
                    primeTensToOnes.Add(6, new int[] { 1, 7 });
                    primeTensToOnes.Add(7, new int[] { 1, 3, 9 });
                    primeTensToOnes.Add(8, new int[] { 3, 9 });
                    primeTensToOnes.Add(9, new int[] { 7 });
                }
                return primeTensToOnes;
            }
        }
        
        /// <summary>
        /// Dictionary mapping the tens of all possible two digit prime number to their ones
        /// </summary>
        public static Dictionary<int, int[]> OnesToTens
        {
            get
            {
                if (primeOnesToTens == null)
                {
                    primeOnesToTens = new Dictionary<int, int[]>();
                    primeOnesToTens.Add(0, new int[] { });
                    primeOnesToTens.Add(1, new int[] { 1, 3, 4, 6, 7 });
                    primeOnesToTens.Add(2, new int[] { 0 });
                    primeOnesToTens.Add(3, new int[] { 0, 1, 2, 4, 5, 7, 8 });
                    primeOnesToTens.Add(4, new int[] { });
                    primeOnesToTens.Add(5, new int[] { 0 });
                    primeOnesToTens.Add(6, new int[] { });
                    primeOnesToTens.Add(7, new int[] { 0, 1, 3, 4, 6, 9 });
                    primeOnesToTens.Add(8, new int[] { });
                    primeOnesToTens.Add(9, new int[] { 1, 2, 5, 7, 8 });
                }
                return primeOnesToTens;
            }
        }

        /// <summary>
        /// Calculates the remaining possible numbers in the region, and updates the possible numbers of the cells in the region accordingly
        /// </summary>
        /// <returns>0 on success. Otherwise error code (1 -> there is no possible number to form a two digit prime, 2 -> a cell in the region has no possible values after update)</returns>
        public override int CalculatePossibleNumbers()
        {
            int tens, ones;
            tens = cells[0].num;
            ones = cells[1].num;
            if (tens != -1)
            {
                if (ones == -1)
                {
                    if (cells[1].UpdatePossible(TensToOnes[tens]) <= 0)
                        return 4;
                }
                else if (!TensToOnes[tens].Contains(ones))
                    return 3;
            }
            else if(ones != -1)
            {
                if (cells[0].UpdatePossible(OnesToTens[ones]) <= 0)
                    return 5;
            }
            return 0;
        }

        /// <summary>
        /// Checks if the values in the cells can possibly form a 2 digit prime
        /// </summary>
        /// <returns>True if they can</returns>
        public override bool CheckDiscrepancies()
        {
            return (cells[0].HasNum && cells[1].HasNum && TensToOnes[cells[0].num].Contains(cells[1].num) ||
                cells[1].HasNum && Cell.PotentialOnes.Contains(cells[1].num) || !cells[1].HasNum);
        }
    }
}
