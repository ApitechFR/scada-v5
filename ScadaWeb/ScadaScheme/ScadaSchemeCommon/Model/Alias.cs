using System;
using System.Xml;

namespace Scada.Scheme.Model
{

    [Serializable]
    public class Alias
    {
        public string Name { get; set; }
        public string AliasTypeName { get; set; }
        public Type AliasType => Type.GetType(AliasTypeName);
        public bool isCnlLinked { get; set; }

        private object _value;
        public object Value { get { return _value; } set { 
                if (value.GetType().Name == AliasTypeName)
                {
                    _value = value;
                }
            } }
        public Alias()
        {
            Name = "";
            AliasTypeName = "string";
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
            xmlElem.AppendElem("AliasType", AliasTypeName);
            xmlElem.AppendElem("Value", Value);
            xmlElem.AppendElem("isCnlLinked", isCnlLinked);
        }
        public void loadFromXml(XmlNode xmlNode)
        {
            if (xmlNode == null)
                throw new ArgumentNullException("xmlNode");

            Name = xmlNode.GetChildAsString("Name");
            AliasTypeName = xmlNode.GetChildAsString("AliasType");
            Value = xmlNode.GetChildAsString("Value");
            isCnlLinked = xmlNode.GetChildAsBool("isCnlLinked");
        }
    }
}
