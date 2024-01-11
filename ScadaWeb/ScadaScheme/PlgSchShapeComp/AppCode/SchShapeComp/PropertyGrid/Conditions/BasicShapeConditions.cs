using System;
using System.Drawing.Design;
using Scada.Scheme.Model.PropertyGrid;
using Scada.Web.Plugins.SchShapeComp.PropertyGrid;
using CM = System.ComponentModel;

namespace Scada.Web.AppCode.SchShapeComp.PropertyGrid.Conditions
{
	[Serializable]
	public class BasicShapeConditions :AdvancedConditions
	{
		public BasicShapeConditions()
			: base()
		{
			IsVisible = true;
			Blinking = BlinkingSpeed.None;
			BackgroundColor = "";
			Height = "";
			Width = "";
		}

		[DisplayName("Height"), Category(Categories.Appearance)]
		public string Height { get; set; }

		[DisplayName("Width"), Category(Categories.Appearance)]
		public string Width { get; set; }

		
		public override void LoadFromXml(System.Xml.XmlNode xmlNode)
		{
			base.LoadFromXml(xmlNode);
			Height = xmlNode.GetChildAsString("Height");
			Width = xmlNode.GetChildAsString("Width");
			
		}

		public override void SaveToXml(System.Xml.XmlElement xmlElem)
		{
			base.SaveToXml(xmlElem);
			xmlElem.AppendElem("Height", Height);
			xmlElem.AppendElem("Width", Width);
			
		}
	}
}