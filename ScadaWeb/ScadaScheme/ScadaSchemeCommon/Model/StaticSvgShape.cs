using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Xml;
using CM = System.ComponentModel;

namespace Scada.Scheme.Model
{
	[Serializable]
	public class StaticSvgShape : BaseComponent
	{
		public StaticSvgShape()
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
