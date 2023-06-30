using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Xml;
using CM = System.ComponentModel;

namespace Scada.Scheme.Model
{
    /// <summary>
    /// Scheme component represents static polygon
    /// </summary>
    [Serializable]
	public class StaticPolygon :BaseComponent
	{
        /// <summary>
        /// Constructor
        /// </summary>
		 public StaticPolygon()
        {
            BackColor = "black";
            RoundedCorners = false;
            NumberOfSides = 4;
		    CornerRadius = 0;
		}


		/// <summary>
		/// Get or set the Sides that define the Sides of polygon
		/// </summary>
		[DisplayName("Sides"), Category(Categories.Appearance)]
		[Description("The Sides that define the polygon.")]
		[CM.Editor(typeof(NumberSelectEditor), typeof(UITypeEditor))]
		[CM.DefaultValue(4)]
		public int NumberOfSides { get; set; }

		/// <summary>
		/// Get or set the corners of the polygon
		/// </summary>
		[DisplayName("Rounded corners"), Category(Categories.Appearance)]
        [Description("If true, the corners of the polygon will be rounded.")]
        [CM.DefaultValue(false)]
        public bool RoundedCorners { get; set; }



		/// <summary>
		/// Get or set the radius of the rounded corners of the polygon
		/// </summary>
		[DisplayName("Corner radius"), Category(Categories.Appearance)]
        [Description("The radius of the rounded corners of the polygon.")]
        [CM.DefaultValue(0)]
        public int CornerRadius { get; set; }

		/// <summary>
		/// Load component configuration from XML node
		/// </summary>
		public override void LoadFromXml(XmlNode xmlNode)
        {
            base.LoadFromXml(xmlNode);
          //  PolyName = xmlNode.GetChildAsString("PolyName");
            NumberOfSides = xmlNode.GetChildAsInt("NumberOfSides");
			//BackgroundColor = xmlNode.GetChildAsString("BackgroundColor");
            RoundedCorners = xmlNode.GetChildAsBool("RoundedCorners");
            CornerRadius = xmlNode.GetChildAsInt("CornerRadius");
        }

		/// <summary>
		/// Save the configuration of a component in an XML node
		/// </summary>
		public override void SaveToXml(XmlElement xmlElem)
        {
            base.SaveToXml(xmlElem);
            xmlElem.AppendElem("NumberOfSides", NumberOfSides);
            xmlElem.AppendElem("RoundedCorners", RoundedCorners);
            xmlElem.AppendElem("CornerRadius", CornerRadius);
        }

       
	}
}
