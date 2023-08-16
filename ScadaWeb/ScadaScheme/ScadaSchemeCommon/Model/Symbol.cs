using Scada.Data.Tables;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Xml;

namespace Scada.Scheme.Model
{
    /// <summary>
    /// Scheme that represents a group of components
    /// </summary>
    [Serializable]
    public class Symbol : ComponentGroup
    {
        public Dictionary<string,int> AliasCnlDictionary { get; set; }

        public List<Alias> AliasList { get; set; }
        public string SymbolId { get; private set; }
        public DateTime LastModificationDate { get; set;}

        public Symbol() :base() {
            SymbolId = Guid.NewGuid().ToString();
            LastModificationDate = DateTime.Now;
            AliasCnlDictionary = new Dictionary<string, int>();
            AliasList = new List<Alias>();
        }

        public override void SaveToXml(XmlElement xmlElem)
        {
            base.SaveToXml(xmlElem);

            xmlElem.AppendElem("SymbolId", SymbolId);
            xmlElem.AppendElem("LastModificationDate", LastModificationDate);
            XmlElement aliasList = xmlElem.OwnerDocument.CreateElement("AliasList");
            foreach(Alias a in AliasList)
            {
                XmlElement alias = xmlElem.OwnerDocument.CreateElement("Alias");
                a.saveToXml(alias);
                AliasCnlDictionary.TryGetValue(a.Name, out int cnlNum);
                alias.AppendElem("CnlNum",cnlNum);
                aliasList.AppendChild(alias);
            }
            xmlElem.AppendChild(aliasList);
        }

        public override void LoadFromXml(XmlNode xmlNode)
        {
            base.LoadFromXml(xmlNode);

            SymbolId = xmlNode.GetChildAsString("SymbolId");
            LastModificationDate = xmlNode.GetChildAsDateTime("LastModificationDate");

            if (xmlNode.SelectSingleNode("AliasList") is XmlNode aliasListNode)
            {
                List<Alias> listTemp = new List<Alias>();

                foreach (XmlNode aliasNode in aliasListNode.ChildNodes)
                {
                    if (aliasNode != null)
                    {
                        Alias alias = new Alias();
                        alias.loadFromXml(aliasNode);
                        listTemp.Add(alias);
                        AliasCnlDictionary.Add(alias.Name, aliasNode.GetChildAsInt("CnlNum"));
                    }
                }
                AliasList.AddRange(listTemp);
            }

        }
    }
}
