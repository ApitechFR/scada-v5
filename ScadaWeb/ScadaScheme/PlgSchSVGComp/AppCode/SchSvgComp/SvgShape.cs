using Scada.Scheme.Model.PropertyGrid;
using Scada.Scheme.Model;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Web;
using System.Xml;
using CM = System.ComponentModel;
using Scada.Web.Plugins.SchSvgComp.PropertyGrid;

namespace Scada.Web.Plugins.SchSvgComp
{
	[Serializable]
	public class SvgShape : BaseComponent
	{
		public SvgShape()
		{
			ShapeType = "Circle";
			BackColor = "black";
		}

		[DisplayName("Shape Type"), Category(Categories.Appearance)]
		[Description("The type of SVG shape.")]
		[CM.Editor(typeof(SvgShapeSelectEditor), typeof(UITypeEditor))]
		[CM.DefaultValue("circle")]
		public string ShapeType { get; set; }

		public override void LoadFromXml(XmlNode xmlNode)
		{
			base.LoadFromXml(xmlNode);
			ShapeType = xmlNode.GetChildAsString("ShapeType");
		}

		public override void SaveToXml(XmlElement xmlElem)
		{
			base.SaveToXml(xmlElem);
			xmlElem.AppendElem("ShapeType", ShapeType);
		}
	}
}