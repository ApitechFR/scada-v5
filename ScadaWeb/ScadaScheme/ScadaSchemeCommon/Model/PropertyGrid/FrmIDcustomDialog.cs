using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scada.Scheme.Model.PropertyGrid
{
    public partial class FrmIDcustomDialog : Form
    {
        public FrmIDcustomDialog()
        {
            InitializeComponent();

            // mise à jour de la TreeView
            updateTreeView(treeView1);
        }

        public void updateTreeView(TreeView treeView)
        {
            List<TreeNodeData> data = new List<TreeNodeData>
            {
                new TreeNodeData
                {
                    Text = "Channels",
                    Children = new List<TreeNodeData>
                    {
                        new TreeNodeData { Text = "All" },
                    }
                },
            };

            FillTreeView(treeView, data);
        }

        private void FillTreeView(TreeView treeView, List<TreeNodeData> data)
        {
            // Effacez tous les nœuds existants
            treeView.Nodes.Clear();

            // Parcourez les données et ajoutez les nœuds au TreeView
            foreach (var item in data)
            {
                // Créez un nouveau nœud avec le texte de l'élément actuel
                TreeNode node = new TreeNode(item.Text);

                // Récursivement, ajoutez les nœuds enfants s'il y en a
                if (item.Children != null)
                {
                    foreach (var child in item.Children)
                    {
                        TreeNode childNode = new TreeNode(child.Text);
                        node.Nodes.Add(childNode);
                    }
                }

                // Ajoutez le nœud au TreeView
                treeView.Nodes.Add(node);
            }
        }
    }

    public class TreeNodeData
    {
        public string Text { get; set; }
        public List<TreeNodeData> Children { get; set; }
    }
}
