using System;
using System.Xml;
using Scada.Scheme.Model.PropertyGrid;

namespace Scada.Web.Plugins.SchShapeComp.PropertyGrid
{
	[Serializable]
	public class DynamicTextConditions : AdvancedConditions
	{
		public DynamicTextConditions()
			: base()
		{
			TextContent = "None";
			FontSize = null;
		}

		[DisplayName("FontSize"),Category(Categories.Appearance)]
		public int? FontSize { get; set; }

		[DisplayName("Text Content"),Category(Categories.Appearance)]
		public string TextContent { get; set; }

		public override void LoadFromXml(XmlNode xmlNode)
		{
			base.LoadFromXml(xmlNode);
			TextContent = xmlNode.GetChildAsString("TextContent");

			XmlNode fontSizeNode = xmlNode.SelectSingleNode("FontSize");
			FontSize = fontSizeNode != null ? (int?)int.Parse(fontSizeNode.InnerText) : null;
		}

		public override void SaveToXml(XmlElement xmlElem)
		{
			base.SaveToXml(xmlElem);
			xmlElem.AppendElem("TextContent", string.IsNullOrEmpty(TextContent) ? "None" : TextContent);
			if (FontSize.HasValue)
			{
				xmlElem.AppendElem("FontSize", FontSize.Value);
			}
		}

	}
}