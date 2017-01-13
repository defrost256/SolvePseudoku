using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolvePseudoku
{
    /// <summary>
    /// Manages the procedure of solving the pseudoku
    /// </summary>
    public class PseudokuSolver
    {
        //public delegate void SolveCycleFinished();

        //TODO: Read from file
        /// <summary>
        /// The cell indexes containing the ones of 2 digit primes
        /// </summary>
        static readonly int[] primeOneCellIdx = { 2, 4, 6, 8, 10, 12, 14, 16 };

        /// <summary>
        /// The list of regions in the pseudoku
        /// </summary>
        List<Region> regions = new List<Region>();
        /// <summary>
        /// The list of all cells in the pseudoku
        /// </summary>
        List<Cell> cells = new List<Cell>();
        /// <summary>
        /// The set of cells which still have no value
        /// </summary>
        HashSet<Cell> unknownCells = new HashSet<Cell>();
        /// <summary>
        /// The number of subcycles to run in a solve cycle
        /// </summary>
        int timeOutCycles;

        /// <summary>
        /// Returns an array containing the values of the cells in order of their indices
        /// </summary>
        public int[] CellNums
        {
            get
            {
                int[] ret = new int[40];
                for(int i = 0; i < 40; i++)
                {
                    ret[i] = cells[i].num;
                }
                return ret;
            }
        }

        /// <summary>
        /// Returns a list of the arrays of possible values each cell can have, in order of their indices
        /// </summary>
        public List<int[]> CellPossibleNums
        {
            get
            {
                List<int[]> ret = new List<int[]>();
                foreach(Cell c in cells)
                {
                    ret.Add(c.PossibleNums);
                }
                return ret;
            }
        }

        //SolveCycleFinished finishedEvent;
        /// <summary>
        /// Initializes the pseudoku table and the solver
        /// Creates the regions, and the initial state of the cells
        /// </summary>
        /// <param name="timeOutCycles">The number of subcycles to run in a solve cycle</param>
        public void Init(int timeOutCycles/*, SolveCycleFinished cycleFinishedEvent*/)
        {
            this.timeOutCycles = timeOutCycles;
            //this.finishedEvent = cycleFinishedEvent;

            for(int i = 0; i < 40; i++)
            {
                cells.Add(new Cell(primeOneCellIdx.Contains(i)));
            }

            //TODO: Read from file

            AddRegion(new int[] { 0, 1, 2, 5, 6, 9, 10, 11, 12, 17, 18, 19, 20, 21, 28, 31, 32, 34, 37, 38 });
            AddRegion(new int[] { 3, 4, 7, 8, 13, 14, 15, 16, 22, 23, 24, 25, 26, 27, 29, 30, 33, 35, 36, 39 });
            AddRegion(new int[] { 0, 1, 2, 3, 4, 7, 10, 13, 14, 17, 18, 22, 24, 26, 28, 29, 30, 31, 32, 33 });
            AddRegion(new int[] { 5, 6, 7, 8, 11, 12, 15, 16, 19, 20, 21, 23, 25, 27, 34, 35, 36, 37, 38, 39 });
            AddRegion(new int[] { 0, 9, 10, 15, 16, 17, 18, 19, 20, 23, 24, 25, 26, 27, 31, 32, 33, 36, 38, 39 });
            AddRegion(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 11, 12, 13, 14, 21, 22, 28, 29, 30, 34, 35, 37 });
            AddRegion(new int[] { 11, 12, 13, 14, 17, 18, 19, 20, 21, 22, 24, 25, 26, 27, 30, 32, 33, 37, 38, 39 });
            AddRegion(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 16, 23, 28, 29, 31, 34, 35, 36 });
            AddRegion(new int[] { 0, 1, 2, 7, 8, 17, 23, 25, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 });
            AddRegion(new int[] { 3, 4, 5, 6, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20, 21, 22, 24, 26, 27 });
            AddRegion(new int[] { 3, 4, 5, 6, 20, 21, 22, 24, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 });
            AddRegion(new int[] { 0, 1, 2, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 23, 25, 26, 27 });

            AddRegion(new int[] { 1, 2 }, true);
            AddRegion(new int[] { 3, 4 }, true);
            AddRegion(new int[] { 9, 10}, true);
            AddRegion(new int[] { 13, 14 }, true);
            AddRegion(new int[] { 11, 12 }, true);
            AddRegion(new int[] { 15, 16 }, true);
            AddRegion(new int[] { 5, 6 }, true);
            AddRegion(new int[] { 7, 8 }, true);

            //TODO: Read from file (or better a UI)

            cells[5].num = 2;
            cells[19].num = 6;
            cells[30].num = 7;
            cells[32].num = 0;
            cells[34].num = 6;
            cells[37].num = 3;
            cells[38].num = 5;

            foreach(Cell c in cells)
            {
                if (!c.HasNum)
                    unknownCells.Add(c);
                else
                    c.UpdatePossible(new int[]{ c.num });
            }
        }

        /// <summary>
        /// Performs a solve cycle consising of a number of subcycles
        /// Each subcycle can update any number of cells as long as there is only one possible number they can have
        /// </summary>
        /// <returns>-1 -> a solution of the pseudoku was found, positive error code (4 -> discrepancy was found at the start of the solve cycle; (1-3) the error code of the region update, if one occured), -2 otherwise</returns>
        public int SolveCycle()
        {
            int finds;
            int cycles = 0;
            int errCode;
            foreach(Region r in regions)
            {
                if (!r.CheckDiscrepancies())
                    return 4;
            }
            do
            {
                while ((finds = CheckCellsForCertainPicks()) == 0 && cycles < timeOutCycles)
                {
                    foreach (Region r in regions)
                        if ((errCode = r.CalculatePossibleNumbers()) != 0)
                            return errCode;
                    cycles++;
                }
                //finishedEvent();
            } while (finds > 0);
            if (unknownCells.Count == 0)
                return -1;
            return -2;
        }

        /// <summary>
        /// Returns the possible values of the cell with the lowest number of possible values
        /// </summary>
        /// <param name="cellIdx">The index of the cell with the lowest number of possible values</param>
        /// <returns>An array of the possible values of the found cell</returns>
        public int[] GetLeastPossibleNums(out int cellIdx)
        {
            int decideIdx = 0;
            Cell decideCell = cells[0], c;
            for (int i = 0; i < 40; i++)
            {
                c = cells[i];
                if (!c.HasNum && c.possibleNumCount < decideCell.possibleNumCount)
                {
                    decideIdx = i;
                    decideCell = c;
                }
            }
            cellIdx = decideIdx;
            return decideCell.PossibleNums;
        }

        /// <summary>
        /// Reinitializes the state of the solver according to the specified values
        /// </summary>
        /// <param name="cellNums">The values the cells in the current state have</param>
        /// <param name="cellPossibleNums">The possible values the cells in the current state can have</param>
        public void LoadState(int[] cellNums, List<int[]> cellPossibleNums)
        {
            unknownCells.Clear();
            for (int i = 0; i < 40; i++)
            {
                cells[i].num = cellNums[i];
                if (!cells[i].HasNum)
                    unknownCells.Add(cells[i]);
                cells[i].ResetPossibleNums(false, cellPossibleNums[i]);
            }
        }

        /// <summary>
        /// Checks if any cell has only one possible value to have, and updates them accordingly
        /// </summary>
        /// <returns>The number of cells updated</returns>
        int CheckCellsForCertainPicks()
        {
            int finds = 0;
            foreach(Cell c in unknownCells)
            {
                if (c.possibleNumCount == 1 && !c.HasNum)
                {
                    c.num = c[0];
                    finds++;
                }
            }
            unknownCells.RemoveWhere(c => c.HasNum);
            return finds;
        }

        /// <summary>
        /// Adds a new region to the pseudoku
        /// Only used internally in constructor
        /// </summary>
        /// <param name="cellIndex">The List of cell indices this region contains</param>
        /// <param name="prime">Indicates if the region should be a PrimeRegion</param>
        void AddRegion(int[] cellIndex, bool prime = false)
        {
            Region newRegion;
            if (prime)
                newRegion = new PrimeRegion();
            else
                newRegion = new BigRegion();

            foreach(int cIdx in cellIndex)
            {
                Cell c = cells[cIdx];
                newRegion.cells.Add(c);
                c.Regions.Add(newRegion);
            }
            regions.Add(newRegion);
        }
    }
}
