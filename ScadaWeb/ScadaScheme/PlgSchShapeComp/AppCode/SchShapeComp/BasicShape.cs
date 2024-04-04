using Scada.Scheme.Model.PropertyGrid;
using Scada.Scheme.Model;
using System;
using System.Drawing.Design;
using System.Xml;
using CM = System.ComponentModel;
using Scada.Web.Plugins.SchShapeComp.PropertyGrid;
using System.Collections.Generic;
using Scada.Scheme.Model.DataTypes;
using Scada.Data.Entities;
using Scada.Web.AppCode.SchShapeComp.PropertyGrid.Conditions;

namespace Scada.Web.Plugins.SchShapeComp
{
	[Serializable]
	public class BasicShape : BaseComponent, IDynamicComponent
	{
		private string shapeType;
		public BasicShape()
		{
			serBinder = PlgUtils.SerializationBinder;
			ShapeType = "Circle";
			BackColor = "Black";
			BorderColor = "Black";
			BorderWidth = 1;
			Action = Actions.None;
			Conditions = new List<BasicShapeConditions>();
			InCnlNum = 0;
			CtrlCnlNum = 0;
			InCnlNumCustom = "NA (0)";
			CtrlCnlNumCustom = "NA (0)";
		}

		[DisplayName("Conditions"), Category(Categories.Behavior)]
		[Description("The conditions for SVG Shape output depending on the value of the input channel.")]
		[CM.DefaultValue(null), CM.TypeConverter(typeof(CollectionConverter))]
		[CM.Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
		public List<BasicShapeConditions> Conditions { get; protected set; }

		// Ajoute ShapeType qui va inclure une logique spéciale lors de la sélection de "Line" pour ajuster SizeX et SizeY à Size(416, 3)
		/// <summary>
		/// shape type 
		/// </summary>
		[DisplayName("Shape Type"), Category(Categories.Appearance)]
		[Description("The type of the shape.")]
		[CM.Editor(typeof(BasicShapeSelectEditor), typeof(UITypeEditor))]
		[CM.DefaultValue("Circle")]
		public string ShapeType
		{
			get=>shapeType;
			set
			{
				shapeType = value;
				if (shapeType == "Line")
				{
					this.Size = new Size(416, 3);
				}
				else
				{
					this.Size = new Size(100, 100);
				}
			}
		}

		[DisplayName("Size"), Category(Categories.Appearance)]
		[Description("The size of the shape.")]
		public Size Size
		{
			get { return base.Size; }
			set { base.Size = value; }
		}

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

		public override void LoadFromXml(XmlNode xmlNode)
		{
			base.LoadFromXml(xmlNode);
			Action = xmlNode.GetChildAsEnum<Actions>("Action");
			Rotation = xmlNode.GetChildAsInt("Rotation");
			InCnlNum = xmlNode.GetChildAsInt("InCnlNum");
			CtrlCnlNum = xmlNode.GetChildAsInt("CtrlCnlNum");
			InCnlNumCustom = xmlNode.GetChildAsString("InCnlNumCustom");
			CtrlCnlNumCustom = xmlNode.GetChildAsString("CtrlCnlNumCustom");
			XmlNode conditionsNode = xmlNode.SelectSingleNode("Conditions");


			if (conditionsNode != null)
			{
				Conditions = new List<BasicShapeConditions>();
				XmlNodeList conditionNodes = conditionsNode.SelectNodes("Condition");
				foreach (XmlNode conditionNode in conditionNodes)
				{
					BasicShapeConditions condition = new BasicShapeConditions { SchemeView = SchemeView };
					condition.LoadFromXml(conditionNode);
					Conditions.Add(condition);
				}
			}
			ShapeType = xmlNode.GetChildAsString("ShapeType");
			
		}

		public override void SaveToXml(XmlElement xmlElem)
		{
			base.SaveToXml(xmlElem);
			XmlElement conditionsElem = xmlElem.AppendElem("Conditions");
			foreach (BasicShapeConditions condition in Conditions)
			{
				XmlElement conditionElem = conditionsElem.AppendElem("Condition");
				condition.SaveToXml(conditionElem);
			}
			xmlElem.AppendElem("Rotation", Rotation);
			xmlElem.AppendElem("ShapeType", ShapeType);
			xmlElem.AppendElem("InCnlNum", InCnlNum);
			xmlElem.AppendElem("CtrlCnlNum", CtrlCnlNum);
			xmlElem.AppendElem("InCnlNumCustom", InCnlNumCustom);
			xmlElem.AppendElem("CtrlCnlNumCustom", CtrlCnlNumCustom);
			xmlElem.AppendElem("Action", Action.ToString());
		
		}

		public override BaseComponent Clone()
		{
			BasicShape cloneComponent = (BasicShape)base.Clone();

			foreach (BasicShapeConditions condition in cloneComponent.Conditions)
			{
				condition.SchemeView = schemeView;
			}

			return cloneComponent;
		}
	}
}