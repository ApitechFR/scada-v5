using Scada.Scheme.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Scada.Scheme.Editor
{
    public partial class FrmAliasSelection : Form
    {
        public Alias selectedAlias { get; private set; }
        public FrmAliasSelection(string PropertyName, List<Alias> availableAlias, int defaultSelectionIndex=-1)
        {
            InitializeComponent();
            label2.Text = PropertyName;
            comboBox1.Items.Clear();
            comboBox1.Items.Add("None");
            foreach (Alias alias in availableAlias)
            {
                comboBox1.Items.Add(alias);
            }
            selectedAlias = defaultSelectionIndex>=0 ? availableAlias[defaultSelectionIndex]:null;
            comboBox1.SelectedIndex = defaultSelectionIndex+1;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedAlias = comboBox1.SelectedIndex == 0 ? null : (Alias)comboBox1.SelectedItem;
        }
    }
}
