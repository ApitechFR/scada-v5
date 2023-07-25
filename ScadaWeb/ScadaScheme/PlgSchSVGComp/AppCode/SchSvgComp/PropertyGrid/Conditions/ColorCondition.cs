using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;
using System;
using System.Drawing.Design;
using System.Xml;
using CM = System.ComponentModel;

namespace Scada.Web.Plugins.SchSvgComp.PropertyGrid
{
	[Serializable]
	public class ColorCondition : Condition
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ColorCondition()
			: base()
		{
			Color = "";
		}


		/// <summary>
		/// Get or set the color displayed when the condition is met.
		/// </summary>
		#region Attributes
		[DisplayName("Color"), Category(Categories.Appearance)]
		[CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		#endregion
		public string Color { get; set; }


		/// <summary>
		/// Load the condition from an XML node.
		/// </summary>
		public override void LoadFromXml(XmlNode xmlNode)
		{
			base.LoadFromXml(xmlNode);
			Color = xmlNode.GetChildAsString("Color");
		}

		/// <summary>
		/// Save the condition to an XML node.
		/// </summary>
		public override void SaveToXml(XmlElement xmlElem)
		{
			base.SaveToXml(xmlElem);
			xmlElem.AppendElem("Color", Color);
		}

		/// <summary>
		/// Clone the object.
		/// </summary>
		public override object Clone()
		{
			Condition clonedCondition = ScadaUtils.DeepClone(this, PlgUtils.SerializationBinder);
			clonedCondition.SchemeView = SchemeView;
			return clonedCondition;
		}
	}
}