using Scada.Scheme.Model;
using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Web;
using System.Xml;
using CM = System.ComponentModel;
using Scada.Web.Plugins.SchShapeComp.PropertyGrid;

namespace Scada.Web.Plugins.SchShapeComp
{
	[Serializable]
	public class Polygon : BaseComponent, IDynamicComponent
	{
		public Polygon()
		{
			serBinder = PlgUtils.SerializationBinder;
			BackColorOnHover = "";
			BorderColorOnHover = "";
			Action = Actions.None;
			Conditions = new List<PolygonCondition>();
			InCnlNum = 0;
			CtrlCnlNum = 0;
			InCnlNumCustom = "NA (0)";
			CtrlCnlNumCustom = "NA (0)";
			BackColor = "black";
			NumberOfSides = 4;
		}


		/// <summary>
		/// Get or set the Sides that define the Sides of polygon
		/// </summary>
		[DisplayName("Sides"), Category(Categories.Appearance)]
		[Description("The Sides that define the polygon.")]
		[CM.Editor(typeof(NumberSelectEditor), typeof(UITypeEditor))]
		[CM.DefaultValue(4)]
		public int NumberOfSides { get; set; }

		[DisplayName("Rotation"), Category(Categories.Appearance)]
		[Description("The rotation angle of the polygon shape in degrees.")]
		[CM.DefaultValue(0)]
		public int Rotation { get; set; }

	
		/// <summary>
		/// Get or set the background color when the mouse pointer hovers over
		/// <summary>
		[DisplayName("Back color on hover"), Category(Categories.Behavior)]
		[Description("The background color of the component when user rests the pointer on it.")]
		[CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		public string BackColorOnHover { get; set; }

		/// <summary>
		/// Get or set Border color
		/// </summary>
		[DisplayName("Border color on hover"), Category(Categories.Behavior)]
		[Description("The border color of the component when user rests the pointer on it.")]
		[CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		public string BorderColorOnHover { get; set; }

		/// <summary>
		/// Get or set the action
		/// </summary>
		[DisplayName("Action"), Category(Categories.Behavior)]
		[Description("The action executed by clicking the left mouse button on the component.")]
		[CM.DefaultValue(Actions.None)]
		public Actions Action { get; set; }

		/// <summary>
		/// Get Polygon output conditions
		/// </summary>
		[DisplayName("Conditions"), Category(Categories.Behavior)]
		[Description("The conditions for polygon output depending on the value of the input channel.")]
		[CM.DefaultValue(null), CM.TypeConverter(typeof(CollectionConverter))]
		[CM.Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
		public List<PolygonCondition> Conditions { get; protected set; }

		/// <summary>
		/// Get or set the input channel number
		/// </summary>
		[CM.Browsable(false)]
		[DisplayName("Input channel"), Category(Categories.Data)]
		[Description("The input channel number associated with the component.")]
		[CM.DefaultValue(0)]
		public int InCnlNum { get; set; }

		/// <summary>
		/// Get or set the input channel number 
		/// </summary>
		[DisplayName("Input channel"), Category(Categories.Data)]
		[Description("The input channel number associated with the component.")]
		[CM.Editor(typeof(IDcustomEditor), typeof(UITypeEditor))]
		public string InCnlNumCustom { get; set; }

		/// <summary>
		/// Get or set the control channel number
		/// </summary>
		[CM.Browsable(false)]
		[DisplayName("Output channel"), Category(Categories.Data)]
		[Description("The output channel number associated with the component.")]
		[CM.DefaultValue(0)]
		public int CtrlCnlNum { get; set; }

		/// <summary>
		/// Get or set the control channel number custom
		/// </summary>
		[DisplayName("Output channel"), Category(Categories.Data)]
		[Description("The output channel number associated with the component.")]
		[CM.Editor(typeof(IDcustomEditor), typeof(UITypeEditor))]
		public string CtrlCnlNumCustom { get; set; }

		/// <summary>
		/// Load component configuration from XML node
		/// </summary>
		public override void LoadFromXml(XmlNode xmlNode)
		{
			base.LoadFromXml(xmlNode);

			BackColorOnHover = xmlNode.GetChildAsString("BackColorOnHover");
			BorderColorOnHover = xmlNode.GetChildAsString("BorderColorOnHover");
			Action = xmlNode.GetChildAsEnum<Actions>("Action");
			Rotation = xmlNode.GetChildAsInt("Rotation");
			XmlNode conditionsNode = xmlNode.SelectSingleNode("Conditions");
			NumberOfSides = xmlNode.GetChildAsInt("NumberOfSides");
			if (conditionsNode != null)
			{
				Conditions = new List<PolygonCondition>();
				XmlNodeList conditionNodes = conditionsNode.SelectNodes("Condition");
				foreach (XmlNode conditionNode in conditionNodes)
				{
					PolygonCondition condition = new PolygonCondition { SchemeView = SchemeView };
					condition.LoadFromXml(conditionNode);
					Conditions.Add(condition);
				}
			}

			InCnlNum = xmlNode.GetChildAsInt("InCnlNum");
			CtrlCnlNum = xmlNode.GetChildAsInt("CtrlCnlNum");
			InCnlNumCustom = xmlNode.GetChildAsString("InCnlNumCustom");
			CtrlCnlNumCustom = xmlNode.GetChildAsString("CtrlCnlNumCustom");
		}
		/// <summary>
		/// Save the configuration of a component in an XML node
		/// </summary>
		/// <param name="xmlElem"></param>
		public override void SaveToXml(XmlElement xmlElem)
		{
			base.SaveToXml(xmlElem);

			xmlElem.AppendElem("BackColorOnHover", BackColorOnHover);
			xmlElem.AppendElem("BorderColorOnHover", BorderColorOnHover);
			xmlElem.AppendElem("Action", Action.ToString());

			XmlElement conditionsElem = xmlElem.AppendElem("Conditions");
			foreach (PolygonCondition condition in Conditions)
			{
				XmlElement conditionElem = conditionsElem.AppendElem("Condition");
				condition.SaveToXml(conditionElem);
			}
			xmlElem.AppendElem("Rotation", Rotation);
			xmlElem.AppendElem("InCnlNum", InCnlNum);
			xmlElem.AppendElem("CtrlCnlNum", CtrlCnlNum);
			xmlElem.AppendElem("InCnlNumCustom", InCnlNumCustom);
			xmlElem.AppendElem("CtrlCnlNumCustom", CtrlCnlNumCustom);
			xmlElem.AppendElem("NumberOfSides", NumberOfSides);
			
			
		}
		/// <summary>
		/// Clone  object
		/// </summary>
		public override BaseComponent Clone()
		{
			Polygon cloneComponent = (Polygon)base.Clone();

			foreach (PolygonCondition condition in cloneComponent.Conditions)
			{
				condition.SchemeView = schemeView;
			}

			return cloneComponent;
		}
	}
}