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
        Texte,
        ChannelID
    }
    internal class Alias
    {
        private Dictionary<AliasTypeEnum, Type> TypeDictionary = new Dictionary<AliasTypeEnum, Type>{
            {AliasTypeEnum.Couleur, typeof(string)},
            {AliasTypeEnum.Nombre, typeof(int) },
            {AliasTypeEnum.Texte, typeof(string) },
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
        };

        public string Name { get; set; }
        public AliasTypeEnum AliasType { get; set; }
        public object Value { get { return Value; } set { 
                //if(value.GetType() == TypeDictionary[AliasType])
                //{
                    if (PredicateDictionnary[AliasType](value))
                    {
                        Value = value;
                    }
                //}
            } }
        public Alias()
        {

        }
    }
}
