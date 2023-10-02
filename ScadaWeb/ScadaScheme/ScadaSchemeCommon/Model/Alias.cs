using System;
using System.Xml;

namespace Scada.Scheme.Model
{
    public class OnUpdateAliasEventArgs : EventArgs
    {
        public Alias OldAlias { get; }
        public Alias NewAlias { get; }

        public OnUpdateAliasEventArgs(Alias oldAlias, Alias newAlias)
        {
            OldAlias = oldAlias;
            NewAlias = newAlias;
        }
    }

    [Serializable]
    public class Alias
    {
        public string Name { get; set; }
        public string AliasTypeName { get; set; }
        public Type AliasType => Type.GetType(AliasTypeName);
        public bool isCnlLinked { get; set; }

        private object _value;
        public object Value { get { return _value; } set { 
                if (value != null && value.GetType().Name == AliasTypeName)
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
            switch (AliasTypeName)
            {
                case "String":
                    Value = xmlNode.GetChildAsString("Value");
                    break;
                case "Int32":
                    Value = xmlNode.GetChildAsInt("Value");
                    break;
                case "Double":
                    Value = xmlNode.GetChildAsInt("Value");
                    break;
                case "Boolean":
                    Value = xmlNode.GetChildAsBool("Value");
                    break;
                default:
                    Value = xmlNode.GetChildAsString("Value");
                    break;
            }
            isCnlLinked = xmlNode.GetChildAsBool("isCnlLinked");
        }
        public Alias Clone()
        {
            Alias alias = new Alias();
            alias.Name = Name;
            alias.AliasTypeName = AliasTypeName;
            alias.Value = Value;
            alias.isCnlLinked = isCnlLinked;
            return alias;
        } 
    }
}
