using Scada.Scheme.Model;
using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;
using Scada.Web.SchShapeComp.PropertyGrid;
using Scada.Web.Plugins.SchShapeComp.PropertyGrid;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Xml;
using CM = System.ComponentModel;


namespace Scada.Web.Plugins.SchShapeComp
{
	[Serializable]
	public class CustomSVG : BaseComponent, IDynamicComponent
	{
		
		public CustomSVG()
		{
			serBinder = PlgUtils.SerializationBinder;
			Action = Actions.None;
			Conditions = new List<AdvancedConditions>();
			InCnlNum = 0;
			CtrlCnlNum = 0;
			InCnlNumCustom = "NA (0)";
			CtrlCnlNumCustom = "NA (0)";
			SvgCode = "";
		}

		private string _svgCode;

		[DisplayName("SVG File"), Category(Categories.Appearance)]
		[Description("The SVG code representing the graphic to be displayed. If empty, no graphic is set.")]
		[CM.Editor(typeof(SVGEditor), typeof(UITypeEditor))]
		[CM.DefaultValue(""), CM.TypeConverter(typeof(SVGTypeConverter))]
		public string SvgCode
		{
			get => _svgCode;
			set
			{
				_svgCode = value;
			}
		}


		[DisplayName("Conditions"), Category(Categories.Behavior)]
		[Description("The conditions for CustomSVG output depending on the value of the input channel.")]
		[CM.DefaultValue(null), CM.TypeConverter(typeof(CollectionConverter))]
		[CM.Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
		public List<AdvancedConditions> Conditions { get; protected set; }


		[DisplayName("Rotation"), Category(Categories.Appearance)]
		[Description("The rotation angle of the SVG shape in degrees.")]
		[CM.DefaultValue(0)]
		public int Rotation { get; set; }



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
		/// Get or set the action
		/// </summary>
		[DisplayName("Action"), Category(Categories.Behavior)]
		[Description("The action executed by clicking the left mouse button on the component.")]
		[CM.DefaultValue(Actions.None)]
		public Actions Action { get; set; }


		#region Attributes
		[CM.Browsable(false)]
		#endregion
		public new string BorderColor { get; set; }

		#region Attributes
		[CM.Browsable(false)]
		#endregion
		public new int BorderWidth { get; set; }


		public override void LoadFromXml(XmlNode xmlNode)
		{
			base.LoadFromXml(xmlNode);

			Action = xmlNode.GetChildAsEnum<Actions>("Action");

			XmlNode conditionsNode = xmlNode.SelectSingleNode("Conditions");

			if (conditionsNode != null)
			{
				Conditions = new List<AdvancedConditions>();
				XmlNodeList conditionNodes = conditionsNode.SelectNodes("Condition");
				foreach (XmlNode conditionNode in conditionNodes)
				{
					AdvancedConditions condition = new AdvancedConditions { SchemeView = SchemeView };
					condition.LoadFromXml(conditionNode);
					Conditions.Add(condition);
				}
			}
			InCnlNum = xmlNode.GetChildAsInt("InCnlNum");
			CtrlCnlNum = xmlNode.GetChildAsInt("CtrlCnlNum");
			InCnlNumCustom = xmlNode.GetChildAsString("InCnlNumCustom");
			CtrlCnlNumCustom = xmlNode.GetChildAsString("CtrlCnlNumCustom");
			SvgCode = xmlNode.GetChildAsString("SVGCode");
			Rotation = xmlNode.GetChildAsInt("Rotation");
		}


		public override void SaveToXml(XmlElement xmlElem)
		{
			base.SaveToXml(xmlElem);


			XmlElement conditionsElem = xmlElem.AppendElem("Conditions");
			foreach (AdvancedConditions condition in Conditions)
			{
				XmlElement conditionElem = conditionsElem.AppendElem("Condition");
				condition.SaveToXml(conditionElem);
			}

			xmlElem.AppendElem("SVGCode", SvgCode);
			xmlElem.AppendElem("Rotation", Rotation);
			xmlElem.AppendElem("InCnlNum", InCnlNum);
			xmlElem.AppendElem("CtrlCnlNum", CtrlCnlNum);
			xmlElem.AppendElem("Action", Action.ToString());
			xmlElem.AppendElem("InCnlNumCustom", InCnlNumCustom);
			xmlElem.AppendElem("CtrlCnlNumCustom", CtrlCnlNumCustom);
		}
		/// <summary>
		/// Clone  object
		/// </summary>
		public override BaseComponent Clone()
		{
			CustomSVG cloneComponent = (CustomSVG)base.Clone();

			foreach (AdvancedConditions condition in cloneComponent.Conditions)
			{
				condition.SchemeView = schemeView;
			}

			return cloneComponent;
		}

	}

}