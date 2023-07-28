using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
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
    [Serializable]
    public class Alias
    {
        private Dictionary<AliasTypeEnum, Type> TypeDictionary = new Dictionary<AliasTypeEnum, Type>{
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
        };

        public string Name { get; set; }
        public AliasTypeEnum AliasType { get; set; }
        public bool isCnlLinked { get; set; }
        public object Value
        {
            get;set; 
            /*
            get { return Value; }
            set
            {
                //if(value.GetType() == TypeDictionary[AliasType])
                //{
                if (PredicateDictionnary[AliasType](value))
                {
                    Value = value;
                }
                //}*/
            }
        
        public Alias()
        {
            Name = "";
            AliasType = AliasTypeEnum.Text;
            isCnlLinked = false;
        }
        public void saveToXml(XmlElement xmlElem)
        {
            if (xmlElem == null)
                throw new ArgumentNullException("xmlElem");

            xmlElem.AppendElem("Name", Name);
            xmlElem.AppendElem("AliasType", AliasType);
            xmlElem.AppendElem("Value", Value);
            xmlElem.AppendElem("isCnlLinked", isCnlLinked);
        }
        public void loadFromXml(XmlNode xmlNode)
        {
            if (xmlNode == null)
                throw new ArgumentNullException("xmlNode");

            Name = xmlNode.GetChildAsString("Name");
            AliasType = (AliasTypeEnum) xmlNode.GetChildAsInt("AliasType");
            Value = xmlNode.GetChildAsString("Value");
            isCnlLinked = xmlNode.GetChildAsBool("isCnlLinked");

        }
    }
}
