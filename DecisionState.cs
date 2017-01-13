using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolvePseudoku
{
    /// <summary>
    /// A Node of the decision tree containing a state of the solution of the Pseudoku
    /// </summary>
    class DecisionState
    {
        /// <summary>
        /// The states originating from this state
        /// </summary>
        List<DecisionState> childDecisions = new List<DecisionState>();
        /// <summary>
        /// The states this state originates from
        /// </summary>
        DecisionState parentDecision;
        public DecisionState Parent { get { return parentDecision; } }
        /// <summary>
        /// Gets a child state of this state based on it's index
        /// </summary>
        /// <param name="i">The index of the child state</param>
        /// <returns></returns>
        public DecisionState this[int i] { get { return childDecisions[i]; } }
        /// <summary>
        /// The number of child states this state has
        /// </summary>
        public int ChildCount { get { return childDecisions.Count; } }
        /// <summary>
        /// The string representation of the series of indices leading to this state
        /// </summary>
        string path;
        public string Path { get { return path; } }

        /// <summary>
        /// The state of the cell values in this state
        /// </summary>
        public int[] cells = new int[40];
        /// <summary>
        /// The state of the possible values the cells in this state can have
        /// </summary>
        public List<int[]> possibleNums = new List<int[]>();

        /// <summary>
        /// Creates a new state for a specified parent.
        /// Initializes the path and adds the state to it's parent
        /// If a parent is null the state is considered the root state
        /// </summary>
        /// <param name="parent">The parent of the created state</param>
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

        /// <summary>
        /// Adds a child state to this state
        /// </summary>
        /// <param name="child">The state to add as a child</param>
        void AddChild(DecisionState child)
        {
            childDecisions.Add(child);
        }

        /// <summary>
        /// Returns a string representation of the series of indices leading to this state
        /// DEPRECATED: Use DecisionState.Path property instead
        /// </summary>
        /// <param name="childState"></param>
        /// <returns></returns>
        [Obsolete("Use DecisionState.Path property instead")]
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
