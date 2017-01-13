using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolvePseudoku
{
    class DecisionState
    {
        List<DecisionState> childDecisions = new List<DecisionState>();
        DecisionState parentDecision;
        public DecisionState Parent { get { return parentDecision; } }
        public DecisionState this[int i] { get { return childDecisions[i]; } }
        public int ChildCount { get { return childDecisions.Count; } }
        string path;

        public int[] cells = new int[40];
        public List<int[]> possibleNums = new List<int[]>();

        public DecisionState(DecisionState parent)
        {
            parentDecision = parent;
            if (parent == null)
                path = "0";
            else
            {
                parent.AddChild(this);
                path = parent.path + "/" + parent.childDecisions.FindIndex(x => x.Equals(this));
            }
        }

        void AddChild(DecisionState child)
        {
            childDecisions.Add(child);
        }

        public string getPath(DecisionState childState = null)
        {
            if(Parent == null)
            {
                return "0/" + (childDecisions.FindIndex(s => s.Equals(childState)));
            }
            if (childState == null)
                return Parent.getPath(this);
            return Parent.getPath(this) + "/" + (childDecisions.FindIndex(s => s.Equals(childState)));
        }
    }
}
