using Scada.Scheme.Model;
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
                Type thisComponentType = ((BaseComponent)(thisNode.Tag)).GetType();
                Type otherComponentType = ((BaseComponent)(otherNode.Tag)).GetType();
                Type componentGroupType = new ComponentGroup().GetType();
                if (thisComponentType == componentGroupType && otherComponentType != componentGroupType)
                {
                    return -1;
                }
                if(otherComponentType == componentGroupType && thisComponentType != componentGroupType)
                {
                    return 1;
                }
            }
            return thisNode.Text.CompareTo(otherNode.Text);
        }
    }
}
