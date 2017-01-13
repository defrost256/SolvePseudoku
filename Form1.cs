using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SolvePseudoku
{
    public partial class Form1 : Form
    {

        Label[] labels = new Label[40];
        Font unknownFont, knownFont, errorFont;

        PseudokuSolver solver = new PseudokuSolver();
        DecisionTree decisions;
        TreeNode currentNode;
        List<int[]> solutions = new List<int[]>();

        public Form1()
        {
            InitializeComponent();
            int tmpIdx;
            foreach(Control c in panel1.Controls)
            {
                if(c.Name.StartsWith("label"))
                {
                    if (!int.TryParse(c.Name.Substring(5), out tmpIdx))
                        throw new Exception("Something's not right");
                    labels[tmpIdx] = c as Label;
                    if (labels[tmpIdx] == null)
                        throw new Exception("Something else isn't right");
                }
            }
            solver.Init(5/*, UpdateCells*/);
            decisions = new DecisionTree(solver.CellNums, solver.CellPossibleNums);
            AddNodeToTreeView("0", IntArrayToString(solver.CellNums));
            unknownFont = new Font(label0.Font, FontStyle.Regular);
            knownFont = new Font(unknownFont, FontStyle.Bold);
            errorFont = new Font(unknownFont, FontStyle.Strikeout);
        }

        private void SolveCycle(object sender = null, EventArgs e = null)
        {
            int[] solution;
            int result;
            string path;
            switch (result = solver.SolveCycle())
            { 
                case -1:    //It's a solution
                    solution = solver.CellNums;
                    decisions.FinalizeBranch(solution, solver.CellPossibleNums);
                    path = decisions.getCurrentStatePath();
                    AddNodeToTreeView(path, IntArrayToString(solution), 0, true);
                    SubmitSolution(solution, path);
                    break;
                case -2:    //It's a regular state
                    int cellIdx;
                    int[] possibleNums;
                    solution = solver.CellNums;
                    possibleNums = solver.GetLeastPossibleNums(out cellIdx);
                    decisions.CreateNewBranches(cellIdx, possibleNums);
                    decisions.FinalizeBranch(solution, solver.CellPossibleNums);
                    path = decisions.getCurrentStatePath();
                    AddNodeToTreeView(path, IntArrayToString(solution));
                    break;
                default:    //ERROR
                    solution = solver.CellNums;
                    decisions.FinalizeBranch(solution, solver.CellPossibleNums);
                    path = decisions.getCurrentStatePath();
                    AddNodeToTreeView(path, IntArrayToString(solution), result);
                    break;
            }
            UpdateView(solution, path);
            DecisionState newDecision;
            if ((newDecision = decisions.GetNextDecision()) == null)
                runSolveCycleButton.Enabled = false;
            else
                solver.LoadState(newDecision.cells, newDecision.possibleNums);
        }

        void AddNodeToTreeView(string pathString, string tooltipString, int errorCode = 0, bool isSolution = false)
        {
            string[] path = pathString.Split('/');
            TreeNodeCollection currentCollection = treeView1.Nodes;
            int tmpChildIdx;
            for(int i = 0; i < path.Length - 1; i++)
            {
                tmpChildIdx = int.Parse(path[i]);
                if (isSolution)
                    currentCollection[tmpChildIdx].NodeFont = knownFont;
                currentCollection = currentCollection[tmpChildIdx].Nodes;
            }
            currentNode = currentCollection.Add(pathString);
            currentNode.ToolTipText = tooltipString;
            if (isSolution)
                currentNode.NodeFont = knownFont;
            if (errorCode != 0)
                currentNode.NodeFont = errorFont;
        }

        public void UpdateView(int[] newCells, string path)
        {
            for(int i = 0; i < 40; i++)
            {
                if (newCells[i] != -1)
                {
                    labels[i].Text = newCells[i].ToString();
                    labels[i].Font = knownFont;
                }
                else
                {
                    labels[i].Text = i.ToString();
                    labels[i].Font = unknownFont;
                }
            }
            CurrentStateLabel.Text = path;
        }

        void SubmitSolution(int[] solution, string path)
        {
            solutions.Add(solution);
            SolutionListBox.Items.Add(path);            
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            DecisionStateLabel.Text = e.Node.ToolTipText;
            UpdateView(StringToIntArray(e.Node.ToolTipText), e.Node.Text);
        }

        private void SolutionListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string path = SolutionListBox.SelectedItem.ToString();
            
        }

        private void ExpandAll(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
        }

        private void CollapseAll(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
        }

        private void SolveAll(object sender, EventArgs e)
        {
            int cycles = (int)CycleNumSelect.Value;
            while(runSolveCycleButton.Enabled && cycles > 0)
            {
                SolveCycle();
                cycles--;
            }
        }

        public static String IntArrayToString(int[] arr)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{ ");
            foreach(int n in arr)
            {
                sb.Append(n + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(" }");
            return sb.ToString();
        }

        public static int[] StringToIntArray(string str)
        {
            string csv = str.Substring(2, str.Length - 4);
            string[] splitCsv = csv.Split(',');
            List<int> ret = new List<int>();
            foreach(string s in splitCsv)
            {
                ret.Add(int.Parse(s));
            }
            return ret.ToArray();
        }
    }
}
