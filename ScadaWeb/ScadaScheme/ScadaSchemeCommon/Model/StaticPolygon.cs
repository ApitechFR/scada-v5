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
		 public StaticPolygon()
        {
            Points = new List<Point>();
            BorderColor = "Gray";
            BorderWidth = 1;
            BackgroundColor = "Transparent";
            RoundedCorners = false;
            CornerRadius = 0;
        }

        [DisplayName("Points"), Category(Categories.Appearance)]
        [Description("Les points qui définissent le polygone.")]
        public List<Point> Points { get; set; }
        public string Poly { get; set; }

        [DisplayName("Background color"), Category(Categories.Appearance)]
        [Description("La couleur de fond du polygone.")]
        [CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        public string BackgroundColor { get; set; }

        [DisplayName("Rounded corners"), Category(Categories.Appearance)]
        [Description("Si vrai, les coins du polygone seront arrondis.")]
        [CM.DefaultValue(false)]
        public bool RoundedCorners { get; set; }

        [DisplayName("Corner radius"), Category(Categories.Appearance)]
        [Description("Le rayon des coins arrondis du polygone.")]
        [CM.DefaultValue(0)]
        public int CornerRadius { get; set; }

        public override void LoadFromXml(XmlNode xmlNode)
        {
            base.LoadFromXml(xmlNode);
            // Points = xmlNode.GetChildAsPointsList("Points");
            Poly = xmlNode.GetChildAsString("poly");
            BackgroundColor = xmlNode.GetChildAsString("BackgroundColor");
            RoundedCorners = xmlNode.GetChildAsBool("RoundedCorners");
            CornerRadius = xmlNode.GetChildAsInt("CornerRadius");
        }

        public override void SaveToXml(XmlElement xmlElem)
        {
            base.SaveToXml(xmlElem);
            xmlElem.AppendElem("Points", Points);
            xmlElem.AppendElem("Poly", Poly);
            xmlElem.AppendElem("BackgroundColor", BackgroundColor);
            xmlElem.AppendElem("RoundedCorners", RoundedCorners);
            xmlElem.AppendElem("CornerRadius", CornerRadius);
        }

        public override string ToString()
        {
            //return "Polygone: " + Points.Count + " points";
            return BuildDisplayName(Poly);
        }
	}
}
