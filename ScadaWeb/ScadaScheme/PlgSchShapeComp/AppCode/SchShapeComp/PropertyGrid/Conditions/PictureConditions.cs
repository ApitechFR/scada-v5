using System;
using System.Xml;
using CM = System.ComponentModel;
using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;
using static Scada.Web.Plugins.SchShapeComp.PropertyGrid.AdvancedConditions;
using Scada.Web.Plugins.SchShapeComp;

namespace Scada.Web.SchShapeComp.PropertyGrid
{
	[Serializable]
	public class PictureConditions : ImageCondition
	{
		public PictureConditions()
			: base()
		{
			IsVisible = true;
			Blinking = BlinkingSpeed.None;
			Rotation = null;
			
		}

		[DisplayName("Rotation"), Category(Categories.Appearance)]
		public int? Rotation { get; set; }


		[DisplayName("Blinking Speed"), Category(Categories.Appearance)]
		public BlinkingSpeed Blinking { get; set; }

		[DisplayName("Visible"), Category(Categories.Appearance)]
		public bool IsVisible { get; set; }


		public override void LoadFromXml(XmlNode xmlNode)
		{
			base.LoadFromXml(xmlNode);
			IsVisible = xmlNode.GetChildAsBool("IsVisible");
			Blinking = xmlNode.GetChildAsEnum<BlinkingSpeed>("Blinking");

			XmlNode rotationNode = xmlNode.SelectSingleNode("Rotation");
			Rotation = rotationNode != null ? (int?)int.Parse(rotationNode.InnerText) : null;
		}

		public override void SaveToXml(XmlElement xmlElem)
		{
			base.SaveToXml(xmlElem);
			if (Rotation.HasValue)
			{
				xmlElem.AppendElem("Rotation", Rotation.Value);
			}
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