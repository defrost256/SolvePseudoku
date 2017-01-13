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
            Label tmpLabel;
            foreach (Control c in panel1.Controls)
            {
                if(c.Name.StartsWith("label"))
                {
                    tmpLabel = c as Label;
                    if (tmpLabel == null)
                        throw new Exception("Label is not a Label");
                    if (!int.TryParse(tmpLabel.Text, out tmpIdx))
                        throw new Exception("Something's not right");
                    tmpLabel.Text = "";
                    labels[tmpIdx] = tmpLabel;
                }
            }
            solver.Init(5);
            decisions = new DecisionTree(solver.CellNums, solver.CellPossibleNums);
            unknownFont = new Font(label0.Font, FontStyle.Regular);
            knownFont = new Font(unknownFont, FontStyle.Bold);
            errorFont = new Font(unknownFont, FontStyle.Strikeout);
        }

        private int[] SolveCycle(out string path)
        {
            int[] solution;
            int result;
            switch (result = solver.SolveCycle())
            { 
                case -1:    //It's a solution
                    solution = solver.CellNums;
                    path = decisions.getCurrentStatePath();
                    AddNodeToTreeView(path, IntArrayToString(solution), 0, true);
                    decisions.FinalizeBranch(solution, solver.CellPossibleNums);
                    SubmitSolution(solution, path);
                    break;
                case -2:    //It's a regular state
                    int cellIdx;
                    int[] possibleNums;
                    solution = solver.CellNums;
                    possibleNums = solver.GetLeastPossibleNums(out cellIdx);
                    decisions.CreateNewBranches(cellIdx, possibleNums);
                    path = decisions.getCurrentStatePath();
                    AddNodeToTreeView(path, IntArrayToString(solution));
                    decisions.FinalizeBranch(solution, solver.CellPossibleNums);
                    break;
                default:    //ERROR
                    solution = solver.CellNums;
                    path = decisions.getCurrentStatePath();
                    AddNodeToTreeView(path, IntArrayToString(solution), result);
                    decisions.FinalizeBranch(solution, solver.CellPossibleNums);
                    break;
            }
            UpdateView(solution, path);
            DecisionState newDecision;
            if ((newDecision = decisions.GetNextDecision()) == null)
                runSolveCycleButton.Enabled = false;
            else
            {
                solver.LoadState(newDecision.cells, newDecision.possibleNums);
            }
            return solution;
        }

        void AddNodeToTreeView(string pathString, string tooltipString, int errorCode = 0, bool isSolution = false)
        {
            TreeNode parentNode = GetParentTreeNodeByPath(pathString);
            pathString = (errorCode != 0 ? errorCode.ToString() + " " : "") + pathString;
            if (parentNode == null)
                currentNode = treeView1.Nodes.Add(pathString);
            else
                currentNode = parentNode.Nodes.Add(pathString);
            currentNode.ToolTipText = tooltipString;
            if (currentNode.Parent != null && LeafViewAliveListBox.Items.Count > 0 && currentNode.Parent.Text == LeafViewAliveListBox.Items[0].ToString())
                LeafViewAliveListBox.Items.RemoveAt(0);
            if (isSolution)
                currentNode.NodeFont = knownFont;
            if (errorCode != 0)
            {
                currentNode.NodeFont = errorFont;
                LeafViewDeadListBox.Items.Add(pathString);
            }
            else
                LeafViewAliveListBox.Items.Add(pathString);
        }

        void UpdateView(int[] newCells, string path)
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
                    labels[i].Text = "";
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

        TreeNode GetParentTreeNodeByPath(string pathStr)
        {
            if (pathStr.Length > 1 && pathStr[1] == ' ')
                pathStr = pathStr.Substring(2);
            string[] path = pathStr.Split('/');
            int tmpChildIdx;
            TreeNodeCollection currentCollection = treeView1.Nodes;
            TreeNode ret = null;
            for (int i = 0; i < path.Length - 1; i++)
            {
                tmpChildIdx = int.Parse(path[i]);
                ret = currentCollection[tmpChildIdx];
                currentCollection = ret.Nodes;
            }
            return ret;
        }

        void SelectInListBox(string path)
        {
            TreeNode node = GetParentTreeNodeByPath(path);
            if (node == null)
                node = treeView1.Nodes[0];
            else
                node = node.Nodes[int.Parse(path[path.Length - 1].ToString())];
            treeView1_AfterSelect(null, new TreeViewEventArgs(node));
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            DecisionStateLabel.Text = e.Node.ToolTipText;
            UpdateView(StringToIntArray(e.Node.ToolTipText), e.Node.Text);
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
            string path = "";
            while(runSolveCycleButton.Enabled && cycles > 1)
            {
                SolveCycle(out path);
                cycles--;
            }
            int[] solution = SolveCycle(out path);
            UpdateView(solution, path);
        }

        private void SolveOne(object sender, EventArgs e)
        {
            string path = "";
            int[] solution = SolveCycle(out path);
            UpdateView(solution, path);
        }

        private void SolutionListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectInListBox(SolutionListBox.SelectedItem.ToString());
        }

        private void LeafViewAliveListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(LeafViewAliveListBox.SelectedItem != null)
                SelectInListBox(LeafViewAliveListBox.SelectedItem.ToString());
        }

        private void LeafViewDeadListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(LeafViewDeadListBox.SelectedItem != null)
                SelectInListBox(LeafViewDeadListBox.SelectedItem.ToString());
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
            bool isError = !str.StartsWith("{");
            string csv = str.Substring(isError ? 3 : 2 , str.Length - (isError ? 5 : 4));
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
