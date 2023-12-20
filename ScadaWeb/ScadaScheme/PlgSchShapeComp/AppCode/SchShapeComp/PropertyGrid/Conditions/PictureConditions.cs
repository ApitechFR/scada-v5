using System;
using System.Xml;
using CM = System.ComponentModel;
using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;
using static Scada.Web.Plugins.SchShapeComp.PropertyGrid.AdvancedCondition;
using Scada.Web.Plugins.SchShapeComp;

namespace Scada.Web.SchShapeComp.PropertyGrid
{
	[Serializable]
	public class PictureConditions : ImageCondition
	{


		[DisplayName("Rotation"), Category(Categories.Appearance)]
		[Description("The rotation angle of the shape in degrees.")]
		[CM.DefaultValue(-1)]
		public int Rotation { get; set; }



		[DisplayName("Blinking Speed"), Category(Categories.Appearance)]
		public BlinkingSpeed Blinking { get; set; }

		[DisplayName("Visible"), Category(Categories.Appearance)]
		public bool IsVisible { get; set; }


		public override void LoadFromXml(XmlNode xmlNode)
		{
			base.LoadFromXml(xmlNode);
			IsVisible = xmlNode.GetChildAsBool("IsVisible");
			Rotation = xmlNode.GetChildAsInt("Rotation");
			Blinking = xmlNode.GetChildAsEnum<BlinkingSpeed>("Blinking");
		}

		public override void SaveToXml(XmlElement xmlElem)
		{
			base.SaveToXml(xmlElem);
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