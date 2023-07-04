using Scada.Scheme.Model;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Scada.Scheme.Editor
{
    public class TreeViewMultipleSelection : TreeView
    {
        protected ArrayList selectedNodes;
        protected TreeNode lastNode, firstNode;

        public TreeViewMultipleSelection()
        {
            selectedNodes = new ArrayList();
        }

        public ArrayList SelectedNodes
        {
            get
            {
                return selectedNodes;
            }
            set
            {
                removePaintFromNodes();
                selectedNodes.Clear();
                selectedNodes = value;
                paintSelectedNodes();
            }
        }
        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown) { return; }
            base.OnBeforeSelect(e);
            bool bControl = (ModifierKeys == Keys.Control);
            bool bShift = (ModifierKeys == Keys.Shift);

            if (bControl && selectedNodes.Contains(e.Node))
            {
                e.Cancel = true;
                removePaintFromNodes();
                selectedNodes.Remove(e.Node);
                paintSelectedNodes();
                return;
            }

            lastNode = e.Node;
            if (!bShift) firstNode = e.Node;
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown) { return; }
            bool bControl = (ModifierKeys == Keys.Control);
            bool bShift = (ModifierKeys == Keys.Shift);

            if (bControl)
            {
                if (!selectedNodes.Contains(e.Node))
                {
                    selectedNodes.Add(e.Node);
                }
                else
                {
                    removePaintFromNodes();
                    selectedNodes.Remove(e.Node);
                }
                paintSelectedNodes();
            }
            else
            {
                if (bShift)
                {
                    Queue myQueue = new Queue();

                    TreeNode uppernode = firstNode;
                    TreeNode bottomnode = e.Node;

                    bool bParent = e.Node.Parent == firstNode;
                    if (!bParent)
                    {
                        bParent = uppernode.Parent == bottomnode;
                        if (bParent)
                        {
                            TreeNode t = uppernode;
                            uppernode = bottomnode;
                            bottomnode = t;
                        }
                    }
                    if (bParent)
                    {
                        TreeNode n = bottomnode;
                        if (uppernode != null)
                        {
                            while (n != uppernode.Parent)
                            {
                                if (!selectedNodes.Contains(n))
                                    myQueue.Enqueue(n);

                                n = n.Parent;
                            }
                        }
                    }
                    else
                    {
                        if ((uppernode.Parent == null && bottomnode.Parent == null) || (uppernode.Parent != null && uppernode.Parent.Nodes.Contains(bottomnode))) // are they siblings ?
                        {
                            int nIndexUpper = uppernode.Index;
                            int nIndexBottom = bottomnode.Index;
                            if (nIndexBottom < nIndexUpper)
                            {
                                TreeNode t = uppernode;
                                uppernode = bottomnode;
                                bottomnode = t;
                                nIndexUpper = uppernode.Index;
                                nIndexBottom = bottomnode.Index;
                            }

                            TreeNode n = uppernode;
                            while (nIndexUpper <= nIndexBottom)
                            {
                                if (!selectedNodes.Contains(n))
                                    myQueue.Enqueue(n);

                                n = n.NextNode;

                                nIndexUpper++;
                            }
                        }
                        else
                        {
                            if (!selectedNodes.Contains(uppernode)) myQueue.Enqueue(uppernode);
                            if (!selectedNodes.Contains(bottomnode)) myQueue.Enqueue(bottomnode);
                        }

                    }

                    selectedNodes.AddRange(myQueue);

                    paintSelectedNodes();
                    firstNode = e.Node;

                }
                else
                {
                    if (selectedNodes != null && selectedNodes.Count > 0)
                    {
                        removePaintFromNodes();
                        selectedNodes.Clear();
                    }
                    selectedNodes.Add(e.Node);
                }
            }
            base.OnAfterSelect(e);
        }
        protected void paintSelectedNodes()
        {
            foreach (TreeNode n in selectedNodes)
            {
                n.BackColor = SystemColors.Highlight;
                n.ForeColor = SystemColors.HighlightText;
            }
        }

        protected void removePaintFromNodes()
        {
            if (selectedNodes.Count == 0) return;

            TreeNode n0 = (TreeNode)selectedNodes[0];
            while (n0.TreeView == null)
            {
                n0 = n0.Parent;
                if(n0 == null)
                {
                    break;
                }
            }

            Color back = n0!= null ? n0.TreeView.BackColor : SystemColors.Info;
            Color fore = n0 != null ? n0.TreeView.ForeColor : SystemColors.InfoText;

            foreach (TreeNode n in selectedNodes)
            {
                n.BackColor = back;
                n.ForeColor = fore;
            }
        }
    }
}
