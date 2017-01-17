using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolvePseudoku
{
    class DecisionTree
    {
        Queue<DecisionState> decisionQueue = new Queue<DecisionState>();
        DecisionState root;

        public DecisionTree(int[] cells, List<int[]> possibleNums)
        {
            root = new DecisionState(null);
            cells.CopyTo(root.cells, 0);
            root.possibleNums = possibleNums;
            decisionQueue.Enqueue(root);
        }

        public void CreateNewBranches(int cellIdx, int[] possibleNums)
        {
            DecisionState tmpState, current = decisionQueue.Peek();
            for (int n = 0; n < possibleNums.Length; n++)
            {
                tmpState = new DecisionState(current);
                for (int i = 0; i < 40; i++)
                {
                    if (i != cellIdx)
                        tmpState.cells[i] = current.cells[i];
                    else
                        tmpState.cells[i] = possibleNums[n];
                }
                tmpState.possibleNums = current.possibleNums;
                //tmpState.possibleNums[cellIdx] = new int[] { possibleNums[n] };
                decisionQueue.Enqueue(tmpState);
            }
        }

        public void FinalizeBranch(int[] cells, List<int[]> possibleNums, bool dead = false)
        {
            DecisionState current = decisionQueue.Dequeue();
            cells.CopyTo(current.cells, 0);
            current.possibleNums = possibleNums;
        }

        public DecisionState GetNextDecision()
        {
            if (decisionQueue.Count == 0)
                return null;
            return decisionQueue.Peek();
        }

        public string getCurrentStatePath()
        {
            if (decisionQueue.Count == 0)
                return "";
            return decisionQueue.Peek().Path;
        }

        public void PrintQueue()
        {
            StringBuilder sb = new StringBuilder(decisionQueue.Count + "{");
            foreach(DecisionState s in decisionQueue)
            {
                sb.Append(s.Path + ", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append("}");
            Console.WriteLine(sb.ToString());
        }
    }
}
