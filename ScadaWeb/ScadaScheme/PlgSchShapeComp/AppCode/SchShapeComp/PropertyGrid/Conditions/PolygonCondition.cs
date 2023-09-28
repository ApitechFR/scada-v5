using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;
using System;
using System.Drawing.Design;
using System.Xml;
using CM = System.ComponentModel;

namespace Scada.Web.Plugins.SchShapeComp.PropertyGrid
{
	/// <summary>
	/// additional properties specific to polygons.
	/// </summary>
	[Serializable]
	public class PolygonCondition : AdvancedCondition
	{

		public PolygonCondition():base() {
			
			Color = "";
		}
		/// <summary>
		/// Property to get or set the number of sides in the polygon. 
		/// This value is user-defined.
		/// </summary>
		[DisplayName("Sides"), Category(Categories.Appearance)]
		[CM.Editor(typeof(NumberSelectEditor), typeof(UITypeEditor))]
		public int Sides { get; set; }

		/// <summary>
		/// Property to get or set the color of the polygon. 
		/// </summary>
		[DisplayName("Color"), Category(Categories.Appearance)]
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