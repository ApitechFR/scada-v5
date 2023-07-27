using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Dictionnary of Alias/cnlId
        /// </summary>
        public Dictionary<Alias, int> AliasList { get; set; }
        public Guid SymbolId {get;}
        public DateTime LastModificationDate { get; set;}

        public Symbol() :base() {
            SymbolId = Guid.NewGuid();
            LastModificationDate = DateTime.Now;
            AliasList = new Dictionary<Alias, int>();
        }

        public override void SaveToXml(XmlElement xmlElem)
        {
            xmlElem.AppendElem("SymbolId", SymbolId);
            xmlElem.AppendElem("LastModificationDate", LastModificationDate);
            XmlElement alias = xmlElem.OwnerDocument.CreateElement("Alias");
            foreach(var a in AliasList)
            {
                alias.AppendElem("Name", a.Key.Name);
                alias.AppendElem("AliasType", a.Key.AliasType);
                alias.AppendElem("IsCnlLinked", a.Key.isCnlLinked);
                if(!a.Key.isCnlLinked)
                {
                    alias.AppendElem("Value", a.Key.Value.ToString());
                }
            }
            xmlElem.AppendChild(alias);
            base.SaveToXml(xmlElem);
        }
    }
}
