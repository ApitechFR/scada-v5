using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;
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
			BackgroundColor = "None";
			IsVisible = true;
			Blinking = BlinkingSpeed.None;
			Rotation = null;

		}

		[DisplayName("Background Color"), Category(Categories.Appearance)]
		[CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		public string BackgroundColor { get; set; }

		[DisplayName("Visible"), Category(Categories.Appearance)]
		public bool IsVisible { get; set; }


		[DisplayName("Rotation"), Category(Categories.Appearance)]
		public int? Rotation { get; set; }


		[DisplayName("Blinking Speed"), Category(Categories.Appearance)]
		public BlinkingSpeed Blinking { get; set; }

		public override void LoadFromXml(XmlNode xmlNode)
		{
			base.LoadFromXml(xmlNode);
			BackgroundColor = xmlNode.GetChildAsString("BackgroundColor");
			IsVisible = xmlNode.GetChildAsBool("IsVisible");
			Blinking = xmlNode.GetChildAsEnum<BlinkingSpeed>("Blinking");

			// If the rotation node is not null, parse the value and assign it to the Rotation property
			XmlNode rotationNode = xmlNode.SelectSingleNode("Rotation");
			Rotation = rotationNode != null ? (int?)int.Parse(rotationNode.InnerText) : null;
		}

		public override void SaveToXml(XmlElement xmlElem)
		{
			base.SaveToXml(xmlElem);
			xmlElem.AppendElem("BackgroundColor", string.IsNullOrEmpty(BackgroundColor) ? "None" : BackgroundColor);
			xmlElem.AppendElem("IsVisible", IsVisible);
			xmlElem.AppendElem("Blinking", Blinking);

			if (Rotation.HasValue)
			{
				xmlElem.AppendElem("Rotation", Rotation.Value);
			}
		}

		public override object Clone()
		{
			Condition clonedCondition = ScadaUtils.DeepClone(this, PlgUtils.SerializationBinder);
			clonedCondition.SchemeView = SchemeView;
			return clonedCondition;
		}
	}
}