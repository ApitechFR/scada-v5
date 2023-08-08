using Scada.Scheme.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scada.Scheme.Editor
{
    public partial class FrmAlias : Form
    {

        Symbol s;

        private Alias _selectedAlias;

        public FrmAlias(Symbol symbol)
        {
            InitializeComponent();

            s = symbol;

            //AddAliasAndSymbole();

            FillListBox();
        }

        //todo: à supprimer
        private void AddAliasAndSymbole()
        {
            Alias al = new Alias();
            al.Name = "alias test";
            al.AliasType = typeof(string);
            al.Value = "value test";
            al.isCnlLinked = false;

            s.AliasList.Add(al);
        }

        private void FillListBox()
        {
            if (listBox1.Items.Count > 0)
                listBox1.Items.Clear();
            foreach (var a in s.AliasList)
                listBox1.Items.Add(a.Name);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Alias alias = new Alias();

            new FrmAliasCreation('C', alias).ShowDialog();
            s.AliasList.Add(alias);
            FillListBox();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_selectedAlias == null)
                MessageBox.Show("Please, select an alias.");
            else
                new FrmAliasCreation('M',_selectedAlias).ShowDialog();

            FillListBox();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex != -1)
                _selectedAlias = s.AliasList[listBox1.SelectedIndex];
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (_selectedAlias == null)
                MessageBox.Show("Please, select an alias.");
            else
                s.AliasList.Remove(_selectedAlias);

            FillListBox();
        }
    }
}
