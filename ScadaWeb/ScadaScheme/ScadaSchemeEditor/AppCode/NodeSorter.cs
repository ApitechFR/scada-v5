using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scada.Scheme.Editor.AppCode
{
    public class NodeSorter:IComparer
    {

        public NodeSorter() { }
        public int Compare(object thisObj, object otherObj)
        {
            TreeNode thisNode = thisObj as TreeNode;
            TreeNode otherNode = otherObj as TreeNode;
            if(thisNode.Tag != null && otherNode.Tag != null)
            {
                return thisNode.Text.CompareTo(otherNode.Text);
            }
            return -thisNode.Text.CompareTo(otherNode.Text);
        }
    }
}
