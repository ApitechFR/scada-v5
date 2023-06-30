﻿using Scada.Scheme.Model.PropertyGrid;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using CM = System.ComponentModel;
namespace Scada.Scheme.Model.DataTypes
{
	/// <summary>
	/// additional properties specific to polygons.
	/// </summary>
	[Serializable]
	public class PolygonCondition :Condition
	{
		/// <summary>
		/// Property to get or set the number of sides in the polygon. 
		/// This value is user-defined.
		/// </summary>
		[DisplayName("Sides"), Category(Categories.Appearance)]
		[Description("The number of sides in the polygon.")]
		[CM.Editor(typeof(NumberSelectEditor), typeof(UITypeEditor))]
		[CM.DefaultValue(3)]
		public int Sides { get; set; }

		/// <summary>
		/// Property to get or set the color of the polygon. 
		/// </summary>
		[DisplayName("Color"), Category(Categories.Appearance)]
		[Description("The color of the polygon.")]
		[CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		public string Color { get; set; }

		/// <summary>
		/// Loads the PolygonCondition from an XML node
		/// </summary>
		public override void LoadFromXml(XmlNode xmlNode)
		{
			base.LoadFromXml(xmlNode);
			Sides = xmlNode.GetChildAsInt("Sides");
			Color = xmlNode.GetChildAsString("Color");
		}

		/// <summary>
		/// Overriding SaveToXml
		/// </summary>
		public override void SaveToXml(XmlElement xmlElem)
		{
			base.SaveToXml(xmlElem);
			xmlElem.AppendElem("Sides", Sides);
			xmlElem.AppendElem("Color", Color);
			
		}

	}
}
