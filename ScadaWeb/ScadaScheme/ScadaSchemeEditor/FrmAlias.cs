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
        //todo: à supprimer
        Symbol s = new Symbol();

        public FrmAlias()
        {
            InitializeComponent();

            AddAliasAndSymbole();

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

            s.Name = "name symbole test";
            s.AliasList.Add(al);
        }

        private void FillListBox()
        {
            foreach (var a in s.AliasList)
                listBox1.Items.Add(a.Name);
        }
    }
}
