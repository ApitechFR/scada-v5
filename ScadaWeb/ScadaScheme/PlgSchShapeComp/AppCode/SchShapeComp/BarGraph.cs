using Scada.Scheme.Model.PropertyGrid;
using Scada.Scheme.Model;
using System;
using System.Drawing.Design;
using System.Xml;
using CM = System.ComponentModel;
using Scada.Web.Plugins.SchShapeComp.PropertyGrid;
using System.Collections.Generic;
using Scada.Scheme.Model.DataTypes;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Scada.Web.Plugins.SchShapeComp
{
	[Serializable]
	public class BarGraph : BaseComponent, IDynamicComponent
	{
		public BarGraph()
		{
			serBinder = PlgUtils.SerializationBinder;
			FillColor = "Blue";
			Conditions = new List<BarGraphConditions>();
			InCnlNum = 0;
			CtrlCnlNum = 0;
			InCnlNumCustom = "NA (0)";
			CtrlCnlNumCustom = "NA (0)";
			BorderWidth = 1;
			BorderColor = "Black";
			Rotation = 0;
			MaxValue = 100;
			MinValue = 0;
		}

		[DisplayName("Conditions"), Category(Categories.Behavior)]
		[Description("The conditions for Bar Graph output depending on the value of the input channel.")]
		[CM.DefaultValue(null), CM.TypeConverter(typeof(CollectionConverter))]
		[CM.Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
		public List<BarGraphConditions> Conditions { get; protected set; }

		[DisplayName("Bar Fill Color"), Category(Categories.Appearance)]
		[Description("The fill color of the Bar Graph.")]
		[CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		[CM.DefaultValue("Blue")]
		public string FillColor { get; set; }

		[DisplayName("Rotation"), Category(Categories.Appearance)]
		[Description("The rotation of the graph")]
		[CM.DefaultValue(0)]
		public int Rotation { get; set; }

		public double maxValue = 100;
		public double minValue = 0;

		[DisplayName("Bar Max Value"), Category(Categories.Appearance)]
		[Description("The max value (decimal) of the Bar Graph.")]
		[CM.DefaultValue(100)]
		public double MaxValue
		{
			get => maxValue;
			set
			{
				if (value > MinValue) 
				{
					maxValue = value;
				}
				else
				{
				MessageBox.Show("MaxValue doit être supérieur à MinValue.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}

		[DisplayName("Bar Min Value"), Category(Categories.Appearance)]
		[Description("The min value (decimal) of the Bar Graph.")]
		[CM.DefaultValue(0)]
		public double MinValue
		{
			get => minValue;
			set
			{
				if (value < MaxValue) 
				{
					minValue = value;
				}
				else
				{
					MessageBox.Show("MinValue doit être inférieur à MaxValue.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}


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
			FillColor = xmlNode.GetChildAsString("FillColor");
			Action = xmlNode.GetChildAsEnum<Actions>("Action");
			InCnlNum = xmlNode.GetChildAsInt("InCnlNum");
			CtrlCnlNum = xmlNode.GetChildAsInt("CtrlCnlNum");
			InCnlNumCustom = xmlNode.GetChildAsString("InCnlNumCustom");
			CtrlCnlNumCustom = xmlNode.GetChildAsString("CtrlCnlNumCustom");
			MaxValue = xmlNode.GetChildAsDouble("MaxValue");
			MinValue = xmlNode.GetChildAsDouble("MinValue");
			Rotation = xmlNode.GetChildAsInt("Rotation");
			XmlNode conditionsNode = xmlNode.SelectSingleNode("Conditions");

			if (conditionsNode != null)
			{
				Conditions = new List<BarGraphConditions>();
				XmlNodeList conditionNodes = conditionsNode.SelectNodes("Condition");
				foreach (XmlNode conditionNode in conditionNodes)
				{
					BarGraphConditions condition = new BarGraphConditions { SchemeView = SchemeView };
					condition.LoadFromXml(conditionNode);
					Conditions.Add(condition);
				}
			}
		}

		public override void SaveToXml(XmlElement xmlElem)
		{
			base.SaveToXml(xmlElem);
			XmlElement conditionsElem = xmlElem.AppendElem("Conditions");
			foreach (BarGraphConditions condition in Conditions)
			{
				XmlElement conditionElem = conditionsElem.AppendElem("Condition");
				condition.SaveToXml(conditionElem);
			}
			xmlElem.AppendElem("FillColor", FillColor);
            if (InCnlNumCustom != null && (InCnlNum == null || InCnlNum == 0) && FindNumberInInCnlNumCustom(InCnlNumCustom) > 0)
            {
                InCnlNum = FindNumberInInCnlNumCustom(InCnlNumCustom);
            }
            xmlElem.AppendElem("InCnlNum", InCnlNum);
            xmlElem.AppendElem("CtrlCnlNum", CtrlCnlNum);
            xmlElem.AppendElem("CtrlCnlNumCustom", CtrlCnlNumCustom);
            xmlElem.AppendElem("InCnlNumCustom", InCnlNumCustom);
            xmlElem.AppendElem("Action", Action.ToString());
			xmlElem.AppendElem("MaxValue", MaxValue);
			xmlElem.AppendElem("MinValue", MinValue);
			xmlElem.AppendElem("Rotation", Rotation);
		}

        public int FindNumberInInCnlNumCustom(string NumCustom)
        {
            int number = 0;

            // Définir l'expression régulière pour extraire les chiffres entre parenthèses
            string pattern = @"\((\d+)\)";

            // Rechercher une correspondance
            Match match = Regex.Match(NumCustom, pattern);

            if (match.Success)
            {
                // Extraire la valeur numérique et la convertir en entier
                number = int.Parse(match.Groups[1].Value);

                return number;
            }
            return number;
        }

        public override BaseComponent Clone()
		{
			BarGraph cloneComponent = (BarGraph)base.Clone();

			foreach (BarGraphConditions condition in cloneComponent.Conditions)
			{
			  condition.SchemeView = schemeView;
			}
			return cloneComponent;
		}
	}
}
