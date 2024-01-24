using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;
using Scada.Web.SchShapeComp.PropertyGrid;
using System;
using System.Drawing.Design;
using System.Xml;
using CM = System.ComponentModel;

namespace Scada.Web.Plugins.SchShapeComp.PropertyGrid
{
	[Serializable]
	public class AdvancedConditions : Condition
	{
		public enum BlinkingSpeed
		{
			None,
			Slow,
			Fast
		}

		public AdvancedConditions()
			: base()
		{
			BackgroundColor = "";
			IsVisible = true;
			Blinking = BlinkingSpeed.None;
			Rotation = 0;
			
		}

		[DisplayName("Background Color"), Category(Categories.Appearance)]
		[CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		public string BackgroundColor { get; set; }

		[DisplayName("Visible"), Category(Categories.Appearance)]
		public bool IsVisible { get; set; }


		[DisplayName("Rotation"), Category(Categories.Appearance)]
		[CM.DefaultValue(0)]
		public int? Rotation { get; set; }
		


		[DisplayName("Blinking Speed"), Category(Categories.Appearance)]
		public BlinkingSpeed Blinking { get; set; }

		public override void LoadFromXml(XmlNode xmlNode)
		{
			base.LoadFromXml(xmlNode);
			BackgroundColor = xmlNode.GetChildAsString("BackgroundColor");
			IsVisible = xmlNode.GetChildAsBool("IsVisible");
			Rotation = xmlNode.GetChildAsInt("Rotation");
			Blinking = xmlNode.GetChildAsEnum<BlinkingSpeed>("Blinking");
		}
		
		public override void SaveToXml(XmlElement xmlElem)
		{
			base.SaveToXml(xmlElem);
			xmlElem.AppendElem("BackgroundColor", BackgroundColor);
			xmlElem.AppendElem("Rotation", Rotation);
			xmlElem.AppendElem("IsVisible", IsVisible);
			xmlElem.AppendElem("Blinking", Blinking);
		}
		
		public override object Clone()
		{
			Condition clonedCondition = ScadaUtils.DeepClone(this, PlgUtils.SerializationBinder);
			clonedCondition.SchemeView = SchemeView;
			return clonedCondition;
		}
	}
}