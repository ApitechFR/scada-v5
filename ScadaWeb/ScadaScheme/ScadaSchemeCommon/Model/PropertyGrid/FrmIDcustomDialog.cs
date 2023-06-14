using Scada.Scheme.Template;
using Scada.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Scada.Scheme;
using System.Runtime.Remoting.Contexts;
using System.Xml;

namespace Scada.Scheme.Model.PropertyGrid
{
    public partial class FrmIDcustomDialog : Form
    {

        private DataTable _dataTable = new DataTable();
        private string _value = "";
        private string _projectPath = "";
        private string _projectXMLPath = "";
        private List<string[]> _lstProperties = new List<string[]>();
        private string[] _xmlNames = { "Cnl.xml", "CnlType.xml", "Device.xml", "Obj.xml" };
        private string _errFolder = "Aucun dossier sélectionné.";

        Scada.Scheme.SchemeContext context = Scada.Scheme.SchemeContext.GetInstance();

        public FrmIDcustomDialog()
        {
            InitializeComponent();

            retrieveBaseXMLDirectory();

            // choosing project path automaticaly
            // TODO
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

        private void fillDataGridView()
        {
            _dataTable.Columns.Add("Name", typeof(string));
            _dataTable.Columns.Add("Tag_Num", typeof(string));
            _dataTable.Columns.Add("Tag_Code", typeof(string));
            _dataTable.Columns.Add("Type", typeof(string));
            _dataTable.Columns.Add("Device", typeof(string));

            foreach (string[] tab in _lstProperties)
            {
                _dataTable.Rows.Add(String.IsNullOrEmpty(tab[0]) ? "" : tab[0], String.IsNullOrEmpty(tab[4]) ? "" : tab[4], String.IsNullOrEmpty(tab[5]) ? "" : tab[5], String.IsNullOrEmpty(tab[6]) ? "" : tab[6], String.IsNullOrEmpty(tab[7]) ? "" : tab[7]);
            }

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

        private void buttonPorjectPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            DialogResult result = folderBrowserDialog.ShowDialog();

            if(result == DialogResult.OK)
            {
                _projectPath = folderBrowserDialog.SelectedPath;
                labelPath.Text = _projectPath;
                Scada.Scheme.SchemeContext.GetInstance().SchemePath = _projectPath;
                _projectXMLPath = Path.Combine(_projectPath, "BaseXML");

                foreach (string name in _xmlNames)
                {
                    xmlReader(name);
                }

                fillDataGridView();
            }
            else
                Console.WriteLine(_errFolder);

        }

        private void xmlReader(string file)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Path.Combine(_projectXMLPath, file));
            XmlNode root = xmlDoc.DocumentElement;

            switch (file)
            {
                case "Cnl.xml":
                    foreach (XmlNode cnlNode in root.SelectNodes("Cnl"))
                    {
                        string[] tab = new string[9];
                        tab[0] = cnlNode.SelectSingleNode("Name").InnerText;
                        tab[1] = cnlNode.SelectSingleNode("CnlTypeID").InnerText;
                        tab[2] = cnlNode.SelectSingleNode("ObjNum").InnerText;
                        tab[3] = cnlNode.SelectSingleNode("DeviceNum").InnerText;
                        tab[4] = cnlNode.SelectSingleNode("TagNum").InnerText;
                        tab[5] = cnlNode.SelectSingleNode("TagCode").InnerText;
                        _lstProperties.Add(tab);
                    }
                    break;

                case "CnlType.xml":
                    foreach (string[] tab in _lstProperties)
                    {
                        foreach (XmlNode cnlTypeNode in root.SelectNodes("CnlType"))
                        {
                            if (tab[1] == cnlTypeNode.SelectSingleNode("CnlTypeID").InnerText)
                                tab[6] = cnlTypeNode.SelectSingleNode("Name").InnerText;
                        }
                    }
                    break;

                case "Device.xml":
                    foreach (string[] tab in _lstProperties)
                    {
                        foreach (XmlNode deviceNode in root.SelectNodes("Device"))
                        {
                            if (tab[3] == deviceNode.SelectSingleNode("DeviceNum").InnerText)
                                tab[7] = deviceNode.SelectSingleNode("Name").InnerText;
                        }
                    }
                    break;


                case "Obj.xml":
                    foreach (string[] tab in _lstProperties)
                    {
                        foreach (XmlNode objNode in root.SelectNodes("Obj"))
                        {
                            if (tab[2] == objNode.SelectSingleNode("ObjNum").InnerText)
                                tab[8] = objNode.SelectSingleNode("Name").InnerText;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void retrieveBaseXMLDirectory()
        {
            if (Directory.Exists(context.SchemePath))
            {
                labelPath.Text = context.SchemePath;
                _projectXMLPath = Path.Combine(context.SchemePath, "BaseXML");

                foreach (string name in _xmlNames)
                {
                    xmlReader(name);
                }
                fillDataGridView();
            }
            else if (File.Exists(context.SchemePath))
            {
                _projectPath = context.SchemePath;

                while (Path.GetFileName(_projectPath) != "Views") //Views Interface
                {
                    string dossierParent = Directory.GetParent(_projectPath).FullName;
                    if (string.IsNullOrEmpty(dossierParent))
                    {
                        return;
                    }
                    _projectPath = dossierParent;
                }
                _projectPath = Directory.GetParent(_projectPath).FullName;
                labelPath.Text = _projectPath;
                _projectXMLPath = Path.Combine(_projectPath, "BaseXML");

                foreach (string name in _xmlNames)
                {
                    xmlReader(name);
                }
                fillDataGridView();
            }
        }
    }

    public class TreeNodeData
    {
        public string Text { get; set; }
        public List<TreeNodeData> Children { get; set; }
    }
}
