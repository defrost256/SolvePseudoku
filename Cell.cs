using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolvePseudoku
{
    /// <summary>
    /// A Cell of the Pseudoku containing a number between 0 and 9 (all inclusive)
    /// </summary>
    public class Cell
    {
        public static readonly int[] PotentialNums = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        public static readonly int[] PotentialPrimes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 };
        public static readonly int[] PotentialOnes = { 2, 3, 5, 7, 9 };

        /// <summary>
        /// The value of the cell
        /// -1 if no value is set yet
        /// </summary>
        public int num = -1;

        /// <summary>
        /// The regions this cell is part of
        /// </summary>
        List<Region> regions = new List<Region>();
        /// <summary>
        /// The remaining possible values this cell can have
        /// </summary>
        List<int> possibleNums;

        /// <summary>
        /// The number of possible values this cell can have
        /// </summary>
        public int possibleNumCount { get { return possibleNums.Count; } }
        /// <summary>
        /// Returns a possible value this cell can have based on it's index
        /// </summary>
        /// <param name="i">The index (the values in the list should be considered in arbitrary order, though it should always be in ascending order)</param>
        /// <returns>The i-th value from the possible values of the cell</returns>
        public int this[int i] { get { return possibleNums[i]; } }
        /// <summary>
        /// The regions this cell is part of
        /// </summary>
        public List<Region> Regions { get { return regions; } }
        /// <summary>
        /// Indicates if the cell has a value already set
        /// </summary>
        public bool HasNum { get { return num != -1; } }
        /// <summary>
        /// Returns an array of the possible values this cell can have
        /// </summary>
        public int[] PossibleNums { get { return possibleNums.ToArray(); } }

        /// <summary>
        /// Creates a new cell with all possible values available
        /// In case of prime one-s these numbers are { 2, 3, 5, 7, 9 }
        /// Otherwise all numbers between 0 and 9 (all inclusive)
        /// </summary>
        /// <param name="ones">Indicates whether the cell is a prime one</param>
        public Cell(bool ones)
        {
            if (ones)
                possibleNums = new List<int>(PotentialOnes);
            else
                possibleNums = new List<int>(PotentialNums);
        }

        /// <summary>
        /// Updates the possible values this cell can have based on a list of constraints
        /// Removes all values from the possible values of the cell which are not present in the constraints
        /// </summary>
        /// <param name="possible">The set of numbers forming the constraint</param>
        /// <returns>The number of possible values this cell can have after the update</returns>
        public int UpdatePossible(IEnumerable<int> possible)
        {
            if (possible.Count() == 10)
                return 10;
            possibleNums.RemoveAll(n => !possible.Contains(n));
            return possibleNums.Count;
        }

        /// <summary>
        /// Resets the possible values to a default value
        /// Works similarly to the constructor unless a set of default possible values is specified
        /// </summary>
        /// <param name="ones">Indicates whether the cell is a prime one</param>
        /// <param name="possible">The default set of possible values</param>
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
