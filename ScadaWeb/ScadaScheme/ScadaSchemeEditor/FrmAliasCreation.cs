using Scada.Scheme.Model;
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
    public partial class FrmAliasCreation : Form
    {
        public char frmType;
        public Alias currentAlias;

        public FrmAliasCreation(char frmType, Alias currentAlias)
        {
            InitializeComponent();

            this.frmType = frmType;
            this.currentAlias = currentAlias;

            // ouverture en création
            if(frmType == 'C')
            {
                textBox1.Text = "";
                textBox2.Text = "";
                comboBox1.SelectedIndex = -1;
                checkBox1.Checked = false;
            }
            else if(frmType == 'M')
            {
                textBox1.Text = currentAlias.Name;
                textBox2.Text = currentAlias.Value == null ? "" : currentAlias.Value.ToString();
                checkBox1.Checked = currentAlias.isCnlLinked;

                int index = GetComboBoxIndexForType(currentAlias.AliasTypeName);

                if (index >= 0) comboBox1.SelectedIndex = index;
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

            if (textBox1.Text == "" || textBox2.Text == "" || comboBox1.SelectedItem == null)
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

                if (currentAlias.isCnlLinked)
                {
                    if (int.TryParse(textBox2.Text, out int result) && selectedValue == "Int32")
                        currentAlias.Value = result;
                    else
                    {
                        MessageBox.Show("Please, enter an int32 value and choose int32 type.");
                        isOKtoClose = false;
                    }
                }

                if(isOKtoClose) this.Close();
            }
        }
    }
}
