using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolvePseudoku
{
    public class Cell
    {
        public static readonly int[] PotentialNums = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        public static readonly int[] PotentialPrimes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 };
        public static readonly int[] PotentialOnes = { 2, 3, 5, 7, 9 };

        public int num = -1;

        List<Region> regions = new List<Region>();
        List<int> possibleNums;

        public int possibleNumCount { get { return possibleNums.Count; } }
        public int this[int i] { get { return possibleNums[i]; } }
        public List<Region> Regions { get { return regions; } }
        public bool HasNum { get { return num != -1; } }
        public int[] PossibleNums { get { return possibleNums.ToArray(); } }

        public Cell(bool ones)
        {
            if (ones)
                possibleNums = new List<int>(PotentialOnes);
            else
                possibleNums = new List<int>(PotentialNums);
        }

        public int UpdatePossible(IEnumerable<int> possible)
        {
            if (possible.Count() == 10)
                return 10;
            possibleNums.RemoveAll(n => !possible.Contains(n));
            return possibleNums.Count;
        }

        public void ResetPossibleNums(bool ones = false, int[] possible = null)
        {
            if(possible == null)
            {
                if (ones)
                    possibleNums = new List<int>(PotentialOnes);
                else
                    possibleNums = new List<int>(PotentialNums);
            }
            else
            {
                possibleNums = new List<int>(possible);
            }
        }
    }
}
