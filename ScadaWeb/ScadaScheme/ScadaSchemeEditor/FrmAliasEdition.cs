using Scada.Scheme.Model;
using Scada.Scheme.Model.PropertyGrid;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scada.Scheme.Editor
{
    public partial class FrmAliasEdition : Form
    {
        public Alias currentAlias;
        private bool isInSymbol;

        public FrmAliasEdition(bool isCreationForm, Alias currentAlias, bool allowRename, bool isInSymbol)
        {
            InitializeComponent();

            this.currentAlias = currentAlias;
            this.isInSymbol = isInSymbol;

            if(isCreationForm)
            {
                textBox1.Text = "";
                textBox2.Text = "";
                comboBox1.SelectedIndex = -1;
                checkBox1.Checked = false;
            }
            else
            {
                textBox1.Text = currentAlias.Name;
                textBox2.Text = currentAlias.Value == null ? "" : currentAlias.Value.ToString();
                checkBox1.Checked = currentAlias.isCnlLinked;
                comboBox1.Enabled = false;
                checkBox1.Enabled = false;
                textBox1.Enabled = allowRename;

                if (checkBox1.Checked)
                {
                    comboBox1.Enabled = false;
                    if (isInSymbol)
                    {
                        textBox2.Enabled = false;
                        btn_browseCnl.Visible = false;
                        textBox2.Text = "0";
                    }
                    else
                        btn_browseCnl.Visible = true;

                }

                int index = GetComboBoxIndexForType(currentAlias.AliasTypeName);
                if (index >= 0) comboBox1.SelectedIndex = index;
                if(checkBox1.Checked) comboBox1.SelectedIndex = -1;
            }
        }

        private int GetComboBoxIndexForType(string type)
        {
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                if (type == comboBox1.Items[i].ToString())
                {
                    return i;
                }
            }
            
            return -1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool isOKtoClose = true;

            if (textBox1.Text == "" || textBox2.Text == "" || (!checkBox1.Checked && comboBox1.SelectedItem == null))
                MessageBox.Show("Please, enter a name, a type and a value.");
            else
            {
                currentAlias.Name = textBox1.Text;

                string selectedValue = comboBox1.SelectedItem == null ? "" : comboBox1.SelectedItem.ToString();
                switch (selectedValue)
                {
                    case "String":
                        currentAlias.AliasTypeName = "String";
                        currentAlias.Value = textBox2.Text.ToString();
                        break;
                    case "Int32":
                        currentAlias.AliasTypeName = "Int32";
                        if (int.TryParse(textBox2.Text, out int result))
                            currentAlias.Value = result;
                        else if (!currentAlias.isCnlLinked)
                        {
                            MessageBox.Show("Please, enter an int32 value.");
                            isOKtoClose = false;
                        }
                        break;
                    case "Double":
                        currentAlias.AliasTypeName = "Double";
                        if (double.TryParse(textBox2.Text, out double resultDouble))
                            currentAlias.Value = resultDouble;
                        else if (!currentAlias.isCnlLinked)
                        {
                            MessageBox.Show("Please, enter a double value.");
                            isOKtoClose = false;
                        }
                        break;
                    case "Boolean":
                        currentAlias.AliasTypeName = "Boolean";
                        if (bool.TryParse(textBox2.Text, out bool resultBool))
                            currentAlias.Value = resultBool;
                        else if (!currentAlias.isCnlLinked)
                        {
                            MessageBox.Show("Please, enter a bool value.");
                            isOKtoClose = false;
                        }
                        break;
                }

                currentAlias.isCnlLinked = checkBox1.Checked;
                if (currentAlias.isCnlLinked && isInSymbol)
                {
                    currentAlias.AliasTypeName = "Int32";
                    if (int.TryParse(textBox2.Text, out int result))
                        currentAlias.Value = result;
                }

                if (isOKtoClose) this.Close();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                comboBox1.Enabled = false;
                comboBox1.SelectedIndex = -1;
                if (isInSymbol)
                {
                    textBox2.Enabled = false;
                    btn_browseCnl.Visible = false;
                    textBox2.Text = "0";
                }
                else
                    btn_browseCnl.Visible = true;
            }
            else
            {
                comboBox1.Enabled = true;
                btn_browseCnl.Visible = false;
                textBox2.Enabled = true;
            }
        }

        private void btn_browseCnl_Click(object sender, EventArgs e)
        {
            FrmIDcustomDialog form = new FrmIDcustomDialog();
            form.ShowDialog();
            textBox2.Text = $"{form.getValue()}";
            currentAlias.AliasTypeName = "String";
            currentAlias.Value = form.getValue().ToString();
        }
    }
}
