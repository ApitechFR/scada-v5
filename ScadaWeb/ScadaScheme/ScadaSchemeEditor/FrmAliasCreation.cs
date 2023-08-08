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

                int index = GetComboBoxIndexForType(currentAlias.AliasType);

                if (index >= 0) comboBox1.SelectedIndex = index;
            }
        }

        private int GetComboBoxIndexForType(Type type)
        {
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                if (type.Name == comboBox1.Items[i].ToString())
                {
                    return i;
                }
            }
            
            return -1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            currentAlias.Name = textBox1.Text;

            string selectedValue = comboBox1.SelectedItem == null ? "" : comboBox1.SelectedItem.ToString();
            switch (selectedValue)
            {
                case "String":
                    currentAlias.AliasType = typeof(string);
                    break;
                case "Int32":
                    currentAlias.AliasType = typeof(int);
                    break;
                case "Double":
                    currentAlias.AliasType = typeof(double);
                    break;
                case "Boolean":
                    currentAlias.AliasType = typeof(bool);
                    break;
            }

            currentAlias.isCnlLinked = checkBox1.Checked;
            currentAlias.Value = textBox2.Text;
        }
    }
}
