using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolvePseudoku
{
    public class PseudokuSolver
    {
        //public delegate void SolveCycleFinished();

        //TODO: Read from file
        static readonly int[] primeOneCellIdx = { 2, 4, 6, 8, 10, 12, 14, 16 };

        List<Region> regions = new List<Region>();
        List<Cell> cells = new List<Cell>();
        HashSet<Cell> unknownCells = new HashSet<Cell>();
        int timeOutCycles;

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
                    c.UpdatePossible(new int[0]);
            }
        }

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

        public void LoadState(int[] cellNums, List<int[]> cellPossibleNums)
        {
            for (int i = 0; i < 40; i++)
            {
                cells[i].num = cellNums[i];
                cells[i].ResetPossibleNums(false, cellPossibleNums[i]);
            }
        }

        int CheckCellsForCertainPicks()
        {
            int finds = 0;
            foreach(Cell c in unknownCells)
            {
                if (c.possibleNumCount == 1)
                {
                    c.num = c[0];
                    c.UpdatePossible(new int[0]);
                    finds++;
                }
            }
            unknownCells.RemoveWhere(c => c.HasNum);
            return finds;
        }

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
