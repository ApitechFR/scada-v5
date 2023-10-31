using Scada.Scheme.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static Scada.Data.Tables.EventTableLight;

namespace Scada.Scheme.Editor
{

    public partial class FrmAliasesList : Form
    {

        Symbol s;
        private Alias _selectedAlias;
        private bool allowCreation;
        public event EventHandler<OnUpdateAliasEventArgs> OnUpdateAlias;

        public FrmAliasesList(Symbol symbol, bool allowCreation)
        {
            InitializeComponent();
            s = symbol;
            this.allowCreation=allowCreation;
            button2.Enabled = allowCreation;
            button3.Enabled = false;
            button5.Enabled = false;
            FillListBox();
            Text = "Aliases for " + s.Name;
        }

        private void FillListBox()
        {
            if (listBox1.Items.Count > 0)
                listBox1.Items.Clear();
            foreach (var a in s.AliasList)
                listBox1.Items.Add(a.Name);

            //_selectedAlias = null;
            button3.Enabled = false;
            button5.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Alias alias = new Alias();
            bool isInSymbol = allowCreation;

            new FrmAliasEdition(true, alias, true, isInSymbol).ShowDialog();
            s.AliasList.Add(alias);
            if (alias.isCnlLinked)
            {
                if (!isInSymbol)
                {
                    //find channel number
                    Match match = Regex.Match(alias.Value.ToString(), @"\((\d+)\)");
                    string matchValue = match.Groups[1].Value;
                    int channelNumber = int.Parse(matchValue);
                }
                s.AliasCnlDictionary.Add(alias.Name, int.Parse(alias.Value.ToString()));
            }
            FillListBox();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Alias oldAlias = _selectedAlias.Clone();
            if (listBox1.SelectedIndex == -1)
                MessageBox.Show("Please, select an alias.");
            else
            {
                bool isInSymbol = allowCreation;
                new FrmAliasEdition(false, _selectedAlias, allowCreation, isInSymbol).ShowDialog();
                if (_selectedAlias.isCnlLinked && !s.AliasCnlDictionary.ContainsKey(_selectedAlias.Name))
                    s.AliasCnlDictionary.Add(_selectedAlias.Name, int.Parse(_selectedAlias.Value.ToString()));
                else if (_selectedAlias.isCnlLinked && s.AliasCnlDictionary.ContainsKey(_selectedAlias.Name) && !isInSymbol)
                {
                    //find channel number
                    Match match = Regex.Match(_selectedAlias.Value.ToString(), @"\((\d+)\)");
                    if(match.Success)
                    {
						string matchValue = match.Groups[1].Value;

                        if(int.TryParse(matchValue, out int channelNumber))
                        {
							s.AliasCnlDictionary[_selectedAlias.Name] = channelNumber;

						}
						else
                        {
                            Console.WriteLine("Error parsing channel number");
                        }
                    }
                    else
                    {
                        Console.WriteLine("match failed");
                    }  
                }
                else if (_selectedAlias.isCnlLinked && s.AliasCnlDictionary.ContainsKey(_selectedAlias.Name) && isInSymbol)
                {
                    s.AliasCnlDictionary[_selectedAlias.Name] = int.Parse(_selectedAlias.Value.ToString());
                }
                FillListBox();  
                OnUpdateAlias?.Invoke(this, new OnUpdateAliasEventArgs(oldAlias, _selectedAlias));
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isRowSelected = listBox1.SelectedIndex != -1;
            button3.Enabled = isRowSelected;
            button5.Enabled = isRowSelected && allowCreation;
            if (isRowSelected)
            {
                _selectedAlias = s.AliasList[listBox1.SelectedIndex];
            }
          
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Alias oldAlias = _selectedAlias;
            if (_selectedAlias == null)
                MessageBox.Show("Please, select an alias.");
            else
            {
                s.AliasList.Remove(_selectedAlias);
                OnUpdateAlias?.Invoke(this, new OnUpdateAliasEventArgs(oldAlias, null));
                FillListBox();
            }
        }
    }
}
