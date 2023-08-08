using System;
using System.Xml;
using System.Drawing.Design;
using CM = System.ComponentModel;
using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;

namespace Scada.Web.Plugins.SchShapeComp.PropertyGrid
{
	[Serializable]
	public class BarGraphCondition : AdvancedCondition
	{
		public enum BarLevel
		{
			Low,
			Min,
			High,
			Medium,
			Max
		}

		public BarGraphCondition() : base()
		{
			FillColor = "";
			Level = BarLevel.Min;
		}

		[DisplayName("Fill Color"), Category(Categories.Appearance)]
		[CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		public string FillColor { get; set; }

		[DisplayName("Bar Level"), Category(Categories.Appearance)]
		public BarLevel Level { get; set; }

		public override void LoadFromXml(XmlNode xmlNode)
		{
			base.LoadFromXml(xmlNode);
			FillColor = xmlNode.GetChildAsString("FillColor");
			Level = xmlNode.GetChildAsEnum<BarLevel>("Level");
		}

		public override void SaveToXml(XmlElement xmlElem)
		{
			base.SaveToXml(xmlElem);
			xmlElem.AppendElem("FillColor", FillColor);
			xmlElem.AppendElem("Level", Level);
		}

		public override object Clone()
		{
			Condition clonedCondition = ScadaUtils.DeepClone(this, PlgUtils.SerializationBinder);
			clonedCondition.SchemeView = SchemeView;
			return clonedCondition;
		}
	}
}