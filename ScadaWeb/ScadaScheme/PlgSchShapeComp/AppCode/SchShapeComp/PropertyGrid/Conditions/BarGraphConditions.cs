using System;
using System.Xml;
using System.Drawing.Design;
using CM = System.ComponentModel;
using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;

namespace Scada.Web.Plugins.SchShapeComp.PropertyGrid
{
	[Serializable]
	public class BarGraphConditions : AdvancedConditions
	{
		
		public BarGraphConditions() : base()
		{
			FillColor = "None";
		}

		[DisplayName("Bar Fill Color"), Category(Categories.Appearance)]
		[CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		public string FillColor { get; set; }

		public override void LoadFromXml(XmlNode xmlNode)
		{
			base.LoadFromXml(xmlNode);
			FillColor = xmlNode.GetChildAsString("FillColor");
		}

		public override void SaveToXml(XmlElement xmlElem)
		{
			base.SaveToXml(xmlElem);
			xmlElem.AppendElem("FillColor", string.IsNullOrEmpty(FillColor) ? "None" : FillColor);
		}

		public override object Clone()
		{
			Condition clonedCondition = ScadaUtils.DeepClone(this, PlgUtils.SerializationBinder);
			clonedCondition.SchemeView = SchemeView;
			return clonedCondition;
		}
	}
}