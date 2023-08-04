using System;
using System.Xml;

namespace Scada.Scheme.Model
{

    [Serializable]
    public class Alias
    {
        public string Name { get; set; }
        public Type AliasType { get; set; }
        public bool isCnlLinked { get; set; }

        private object _value;
        public object Value { get { return _value; } set { 
                if (value.GetType() == AliasType)
                {
                    _value = value;
                }
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
            AliasType = Type.GetType(xmlNode.GetChildAsString("AliasType"));
            Value = xmlNode.GetChildAsString("Value");
            isCnlLinked = xmlNode.GetChildAsBool("isCnlLinked");

        }
    }
}
