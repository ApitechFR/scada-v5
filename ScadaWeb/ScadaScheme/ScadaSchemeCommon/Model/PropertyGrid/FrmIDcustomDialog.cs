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
        private int _numValue;
        private string _projectPath = "";
        private string _projectXMLPath = "";
        private List<string[]> _lstProperties = new List<string[]>();
        private string[] _xmlNames = { "Cnl.xml", "CnlType.xml", "Device.xml", "Obj.xml" };
        private string _errFolder = "Aucun dossier sélectionné.";
        private string _errProject = "Veuillez sélectionner un dossier projet valide.";
        private string[] tabElement = {"By devices", "By objects"};


        Scada.Scheme.SchemeContext context = Scada.Scheme.SchemeContext.GetInstance();

        public FrmIDcustomDialog()
        {
            InitializeComponent();

            retrieveBaseXMLDirectory();
        }

        public void updateTreeView(TreeView treeView)
        {
            TreeNode parentNode = new TreeNode("Channels");
            treeView1.Nodes.Add(parentNode);

            TreeNode allNode = new TreeNode("All");
            parentNode.Nodes.Add(allNode);

            int count = 0;
            foreach(string s in tabElement)
            {
                TreeNode node = new TreeNode(s);
                parentNode.Nodes.Add(node);

                List<string> list = new List<string>();
                foreach (string[] tab in _lstProperties)
                {
                    if (!String.IsNullOrEmpty(tab[8+count]) && !list.Contains(tab[8+count]))
                        list.Add(tab[8+count]);
                }
                foreach (string device in list)
                {
                    TreeNode n = new TreeNode(device);
                    node.Nodes.Add(n);
                }
                count++;
            }
        }

        private void fillDataGridView()
        {
            _dataTable.Columns.Add("NumCln", typeof(int));
            _dataTable.Columns.Add("Name", typeof(string));
            _dataTable.Columns.Add("Tag_Num", typeof(string));
            _dataTable.Columns.Add("Tag_Code", typeof(string));
            _dataTable.Columns.Add("Type", typeof(string));
            _dataTable.Columns.Add("Device", typeof(string));
            _dataTable.Columns.Add("Object", typeof(string));

            foreach (string[] tab in _lstProperties)
            {
                _dataTable.Rows.Add(tab[0] == null ? "" : tab[0],String.IsNullOrEmpty(tab[1]) ? "" : tab[1], String.IsNullOrEmpty(tab[5]) ? "" : tab[5], String.IsNullOrEmpty(tab[6]) ? "" : tab[6], String.IsNullOrEmpty(tab[7]) ? "" : tab[7], String.IsNullOrEmpty(tab[8]) ? "" : tab[8], String.IsNullOrEmpty(tab[9]) ? "" : tab[9]);
            }

            dataGridView1.DataSource = _dataTable;
            dataGridView1.Columns[dataGridView1.Columns.Count - 1].Visible = false;
            dataGridView1.Columns[0].Visible = false;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            search();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 0)
            {
                _numValue = int.Parse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                _value = $"({_numValue}) {dataGridView1.SelectedRows[0].Cells[1].Value.ToString()}";
            }
            else _value = "NA (0)";
            DialogResult = DialogResult.OK;

        }

        public string getValue(){return _value;}

        public int getNumValue() { return _numValue; }

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
            dataView.RowFilter = string.Format(@"Name LIKE '%{0}%' OR Tag_Num LIKE '%{0}%' OR Tag_Code LIKE '%{0}%' OR Type LIKE '%{0}%' OR Device LIKE '%{0}%'", textBox1.Text);
            dataGridView1.DataSource = dataView;
        }

        private void buttonPorjectPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            DialogResult result = folderBrowserDialog.ShowDialog();

            if(result == DialogResult.OK)
            {
                _projectPath = folderBrowserDialog.SelectedPath;
                if (Directory.Exists(Path.Combine(_projectPath, "BaseXML")))
                {
                    textBox2.Text = _projectPath;
                    Scada.Scheme.SchemeContext.GetInstance().SchemePath = _projectPath;
                    _projectXMLPath = Path.Combine(_projectPath, "BaseXML");

                    foreach (string name in _xmlNames)
                    {
                        xmlReader(name);
                    }

                    fillDataGridView();
                    updateTreeView(treeView1);
                }
                else MessageBox.Show(_errProject);
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
                        string[] tab = new string[10];
                        tab[0] = cnlNode.SelectSingleNode("CnlNum")?.InnerText;
                        tab[1] = cnlNode.SelectSingleNode("Name")?.InnerText;
                        tab[2] = cnlNode.SelectSingleNode("CnlTypeID")?.InnerText;
                        tab[3] = cnlNode.SelectSingleNode("ObjNum")?.InnerText;
                        tab[4] = cnlNode.SelectSingleNode("DeviceNum")?.InnerText;
                        tab[5] = cnlNode.SelectSingleNode("TagNum")?.InnerText;
                        tab[6] = cnlNode.SelectSingleNode("TagCode")?.InnerText;
                        _lstProperties.Add(tab);
                    }
                    break;

                case "CnlType.xml":
                    foreach (string[] tab in _lstProperties)
                    {
                        foreach (XmlNode cnlTypeNode in root.SelectNodes("CnlType"))
                        {
                            if (tab[2] == cnlTypeNode.SelectSingleNode("CnlTypeID").InnerText)
                                tab[7] = cnlTypeNode.SelectSingleNode("Name").InnerText;
                        }
                    }
                    break;

                case "Device.xml":
                    foreach (string[] tab in _lstProperties)
                    {
                        foreach (XmlNode deviceNode in root.SelectNodes("Device"))
                        {
                            if (tab[4] == deviceNode.SelectSingleNode("DeviceNum").InnerText)
                                tab[8] = deviceNode.SelectSingleNode("Name").InnerText;
                        }
                    }
                    break;


                case "Obj.xml":
                    foreach (string[] tab in _lstProperties)
                    {
                        foreach (XmlNode objNode in root.SelectNodes("Obj"))
                        {
                            if (tab[3] == objNode.SelectSingleNode("ObjNum").InnerText)
                                tab[9] = objNode.SelectSingleNode("Name").InnerText;
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
                if (Directory.Exists(Path.Combine(context.SchemePath, "BaseXML")))
                {
                    textBox2.Text = context.SchemePath;
                    _projectXMLPath = Path.Combine(context.SchemePath, "BaseXML");

                    foreach (string name in _xmlNames)
                    {
                        xmlReader(name);
                    }
                    fillDataGridView();
                    updateTreeView(treeView1);
                }
            }
            else if (File.Exists(context.SchemePath))
            {
                _projectPath = context.SchemePath;
                _projectPath = Path.GetDirectoryName(_projectPath);

                if (Directory.GetParent(_projectPath) != null)
                {
                    while (true)
                    {
                        string[] projectFiles = Directory.GetFiles(_projectPath, "*.rsproj");

                        if (projectFiles.Length > 0)
                        {
                            textBox2.Text = _projectPath;
                            if (Directory.Exists(Path.Combine(_projectPath, "BaseXML")))
                            {
                                _projectXMLPath = Path.Combine(_projectPath, "BaseXML");

                                foreach (string name in _xmlNames)
                                {
                                    xmlReader(name);
                                }
                                fillDataGridView();
                                updateTreeView(treeView1);
                            }
                            break;
                        }

                        if (_projectPath == Path.GetPathRoot(_projectPath))
                        {
                            break;
                        }
                        _projectPath = Directory.GetParent(_projectPath).FullName;
                    }

                }
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(e.Node.Parent != null && tabElement.Contains(e.Node.Parent.Text) && e.Node.Parent.Text.Contains("devices"))
            {
                string selectedDevice = e.Node.Text;

                DataView dataView = _dataTable.DefaultView;
                dataView.RowFilter = string.Format(@"Device LIKE '%{0}%'",selectedDevice);
                dataGridView1.DataSource = dataView;
            }

            if (e.Node.Parent != null && tabElement.Contains(e.Node.Parent.Text) && e.Node.Parent.Text.Contains("objects"))
            {
                string selectedObject = e.Node.Text;

                DataView dataView = _dataTable.DefaultView;
                dataView.RowFilter = string.Format(@"Object LIKE '%{0}%'", selectedObject);
                dataGridView1.DataSource = dataView;
            }

            if (e.Node.Parent != null && e.Node.Parent.Text == "Channels")
            {
                DataView dataView = _dataTable.DefaultView;
                dataView.RowFilter = string.Format(@"Device LIKE '%{0}%'", "");
                dataGridView1.DataSource = dataView;
            }
        }
    }

    public class TreeNodeData
    {
        public string Text { get; set; }
        public List<TreeNodeData> Children { get; set; }
    }
}
