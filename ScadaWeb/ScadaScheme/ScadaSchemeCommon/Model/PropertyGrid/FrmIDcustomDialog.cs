using Scada.Scheme.Template;
using Scada.Web;
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

        private DataTable _dataTable = new DataTable();
        private string _value = ""; 

        public FrmIDcustomDialog()
        {
            InitializeComponent();
            
            // mise à jour de la TreeView
            updateTreeView(treeView1);

            //test
            fillTestDataGrid();

            string pathXml = AppDirs.DefWebAppDir;
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

            fillTreeView(treeView, data);
        }

        private void fillTreeView(TreeView treeView, List<TreeNodeData> data)
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

        //test
        private void fillTestDataGrid()
        {
            _dataTable.Columns.Add("Name", typeof(string));
            _dataTable.Columns.Add("Tag_Num", typeof(string));
            _dataTable.Columns.Add("Tag_Code", typeof(string));
            _dataTable.Columns.Add("Type", typeof(string));
            _dataTable.Columns.Add("Device", typeof(string));

            // Ajouter les deux lignes de données
            _dataTable.Rows.Add("Vanne A145", "12345", "Vanne A", "Input", "Simulator");
            _dataTable.Rows.Add("Mesure h2s ouvrage", "20500", "211_29_ait_001", "Output", "Device3");

            dataGridView1.DataSource = _dataTable;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            search();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 0)
                _value = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            else _value = "";
            DialogResult = DialogResult.OK;

        }

        public string getValue()
        {
            return _value;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                search();
            }
        }

        private void search()
        {
            DataView dataView = _dataTable.DefaultView;
            dataView.RowFilter = string.Format(@"Name LIKE '%{0}%'", textBox1.Text);
            dataGridView1.DataSource = dataView;
        }
    }

    public class TreeNodeData
    {
        public string Text { get; set; }
        public List<TreeNodeData> Children { get; set; }
    }
}
