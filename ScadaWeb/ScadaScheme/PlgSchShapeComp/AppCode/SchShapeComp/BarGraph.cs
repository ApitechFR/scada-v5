using Scada.Scheme.Model.PropertyGrid;
using Scada.Scheme.Model;
using System;
using System.Drawing.Design;
using System.Xml;
using CM = System.ComponentModel;
using Scada.Web.Plugins.SchShapeComp.PropertyGrid;
using System.Collections.Generic;
using Scada.Scheme.Model.DataTypes;

namespace Scada.Web.Plugins.SchShapeComp
{
	[Serializable]
	public class BarGraph : BaseComponent, IDynamicComponent
	{
		public BarGraph()
		{
			serBinder = PlgUtils.SerializationBinder;
			BarColor = "blue";
			//MaxValue = 100;
			//MinValue = 0;
			//CurrentValue = 50;
			Value = 10;
			Conditions = new List<AdvancedCondition>();
			InCnlNum = 0;
			CtrlCnlNum = 0;
			InCnlNumCustom = "NA (0)";
			CtrlCnlNumCustom = "NA (0)";
		}

		[DisplayName("Conditions"), Category(Categories.Behavior)]
		[Description("The conditions for Bar Graph output depending on the value of the input channel.")]
		[CM.DefaultValue(null), CM.TypeConverter(typeof(CollectionConverter))]
		[CM.Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
		public List<AdvancedCondition> Conditions { get; protected set; }


		[DisplayName("Bar Color"), Category(Categories.Appearance)]
		[Description("The color of the Bar Graph.")]
		[CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[CM.DefaultValue("blue")]
		public string BarColor { get; set; }

		//[DisplayName("Max Value"), Category(Categories.Data)]
		//[Description("The maximum value represented by the Bar Graph.")]
		//[CM.DefaultValue(100)]
		//public double MaxValue { get; set; }

		[DisplayName("Bar Value"), Category(Categories.Appearance)]
		[Description("The minimum value represented by the Bar Graph.")]
		[CM.DefaultValue(10)]
		public double Value { get; set; }

		//[DisplayName("Current Value"), Category(Categories.Data)]
		//[Description("The current value represented by the Bar Graph.")]
		//[CM.DefaultValue(50)]
		//public double CurrentValue { get; set; }

		/// <summary>
		/// Get or set the action
		/// </summary>
		[DisplayName("Action"), Category(Categories.Behavior)]
		[Description("The action executed by clicking the left mouse button on the component.")]
		[CM.DefaultValue(Actions.None)]
		public Actions Action { get; set; }

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

		public override void LoadFromXml(XmlNode xmlNode)
		{
			base.LoadFromXml(xmlNode);
			Value = xmlNode.GetChildAsDouble("Value");
		//	MinValue = xmlNode.GetChildAsDouble("MinValue");
			//CurrentValue = xmlNode.GetChildAsDouble("CurrentValue");
			BarColor = xmlNode.GetChildAsString("BarColor");
			Action = xmlNode.GetChildAsEnum<Actions>("Action");
			InCnlNum = xmlNode.GetChildAsInt("InCnlNum");
			CtrlCnlNum = xmlNode.GetChildAsInt("CtrlCnlNum");
			InCnlNumCustom = xmlNode.GetChildAsString("InCnlNumCustom");
			CtrlCnlNumCustom = xmlNode.GetChildAsString("CtrlCnlNumCustom");
			XmlNode conditionsNode = xmlNode.SelectSingleNode("Conditions");

			if (conditionsNode != null)
			{
				Conditions = new List<AdvancedCondition>();
				XmlNodeList conditionNodes = conditionsNode.SelectNodes("Condition");
				foreach (XmlNode conditionNode in conditionNodes)
				{
					AdvancedCondition condition = new AdvancedCondition { SchemeView = SchemeView };
					condition.LoadFromXml(conditionNode);
					Conditions.Add(condition);
				}
			}
		}

		public override void SaveToXml(XmlElement xmlElem)
		{
			base.SaveToXml(xmlElem);
			
			//xmlElem.AppendElem("MaxValue", MaxValue);
			xmlElem.AppendElem("Value", Value);
			//xmlElem.AppendElem("CurrentValue", CurrentValue);

			XmlElement conditionsElem = xmlElem.AppendElem("Conditions");
			foreach (AdvancedCondition condition in Conditions)
			{
				XmlElement conditionElem = conditionsElem.AppendElem("Condition");
				condition.SaveToXml(conditionElem);
			}
			xmlElem.AppendElem("BarColor", BarColor);
			xmlElem.AppendElem("InCnlNum", InCnlNum);
			xmlElem.AppendElem("CtrlCnlNum", CtrlCnlNum);
			xmlElem.AppendElem("InCnlNumCustom", InCnlNumCustom);
			xmlElem.AppendElem("CtrlCnlNumCustom", CtrlCnlNumCustom);
			xmlElem.AppendElem("Action", Action.ToString());
		}

		public override BaseComponent Clone()
		{
			BarGraph cloneComponent = (BarGraph)base.Clone();

			foreach (AdvancedCondition condition in cloneComponent.Conditions)
			{
				condition.SchemeView = schemeView;
			}

			return cloneComponent;
		}
	}
}
