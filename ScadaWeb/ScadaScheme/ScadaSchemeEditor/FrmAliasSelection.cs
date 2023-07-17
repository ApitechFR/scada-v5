using Scada.Scheme.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Scada.Scheme.Editor
{
    public partial class FrmAliasSelection : Form
    {
        public FrmAliasSelection(string PropertyName, List<Alias> availableAlias)
        {
            InitializeComponent();
            label2.Text = PropertyName;
            comboBox1.Items.Clear();
            foreach (Alias alias in availableAlias)
            {
                comboBox1.Items.Add(alias);
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
