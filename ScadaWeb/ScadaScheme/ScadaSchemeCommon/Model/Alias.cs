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
    public enum AliasTypeEnum
    {
        Couleur,
        Nombre,
        Text,
        ChannelID
    }
    public class Alias
    {
        public Dictionary<AliasTypeEnum, Type> TypeDictionary = new Dictionary<AliasTypeEnum, Type>{
            {AliasTypeEnum.Couleur, typeof(string)},
            {AliasTypeEnum.Nombre, typeof(int) },
            {AliasTypeEnum.Text, typeof(string) },
            {AliasTypeEnum.ChannelID, typeof(int) },
        };
        private Dictionary<AliasTypeEnum, Func<object, bool>> PredicateDictionnary = 
            new Dictionary<AliasTypeEnum, Func<object, bool>>{
            {AliasTypeEnum.Couleur, (object colorValue)=>{
                try
                {
                    if(colorValue==null)
                    {
                        return false;
                    }
                    string colorString = (string)colorValue;
                    var regex = @"#+([A-Fa-f0-9]){6}";
                    return Regex.Match(colorString, regex).ToString().Length == colorString.Length ;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                return false;
            }},
            {AliasTypeEnum.Text, (object textValue)=>{
                bool isValid = false;
                try
                {
                    isValid = textValue.ToString() == (string)textValue;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
                return isValid;
            }},
        };

        public string Name { get; set; }
        public AliasTypeEnum AliasType { get; set; }
        public bool isCnlLinked { get; set; }

        private object _value;
        public object Value { get { return _value; } set { 
                //if(value.GetType() == TypeDictionary[AliasType])
                //{
                    if (PredicateDictionnary.Keys.Contains(AliasType) && PredicateDictionnary[AliasType](value))
                    {
                    _value = value;
                    }
                //}
            } }
        public Alias()
        {
            Name = "";
            AliasType = AliasTypeEnum.Text;
            isCnlLinked = false;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
