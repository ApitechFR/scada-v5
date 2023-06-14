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
        }

        public void updateTreeView(TreeView treeView)
        {
            TreeNode parentNode = new TreeNode("Channels");
            treeView1.Nodes.Add(parentNode);

            TreeNode allNode = new TreeNode("All");
            parentNode.Nodes.Add(allNode);

            TreeNode devicesNode = new TreeNode("By devices");
            parentNode.Nodes.Add(devicesNode);

            TreeNode objectsNode = new TreeNode("By objects");
            parentNode.Nodes.Add(objectsNode);

            //récuperation des devices
            List<string> lstNameDevices = new List<string>();
            foreach (string[] tab in _lstProperties)
            {
                if (!String.IsNullOrEmpty(tab[7]) && !lstNameDevices.Contains(tab[7]))
                    lstNameDevices.Add(tab[7]);
            }
            foreach(string device in lstNameDevices)
            {
                TreeNode node = new TreeNode(device);
                devicesNode.Nodes.Add(node);
            }

            //récuperation des objets
            List<string> lstNameObjets = new List<string>();
            foreach (string[] tab in _lstProperties)
            {
                if (!String.IsNullOrEmpty(tab[8]) && !lstNameObjets.Contains(tab[8]))
                    lstNameObjets.Add(tab[8]);
            }
            foreach (string objet in lstNameObjets)
            {
                TreeNode node = new TreeNode(objet);
                objectsNode.Nodes.Add(node);
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
                updateTreeView(treeView1);
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
                updateTreeView(treeView1);
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
                updateTreeView(treeView1);
            }
        }
    }

    public class TreeNodeData
    {
        public string Text { get; set; }
        public List<TreeNodeData> Children { get; set; }
    }
}
