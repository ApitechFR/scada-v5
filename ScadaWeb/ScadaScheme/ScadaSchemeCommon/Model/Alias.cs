using ExCSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Schema;

namespace Scada.Scheme.Model
{
   
    public class Alias
    {
        public string Name { get; set; }
        public Type AliasType { get; set; }
        public bool isCnlLinked { get; set; }

        private object _value;
        public object Value { get { return _value; } set { 
                //if(value.GetType() == TypeDictionary[AliasType])
                //{
                    if (value.GetType() == AliasType)
                    {
                        _value = value;
                    }
                //}
            } }
        public Alias()
        {
            Name = "";
            AliasType = typeof(string);
            isCnlLinked = false;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
