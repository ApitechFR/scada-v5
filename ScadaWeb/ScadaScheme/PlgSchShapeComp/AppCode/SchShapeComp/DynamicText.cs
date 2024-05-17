﻿using System;
using System.Xml;
using Scada.Scheme.Model;
using System.Drawing.Design;
using CM = System.ComponentModel;
using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;
using Scada.Web.Plugins.SchShapeComp.PropertyGrid;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Scada.Web.Plugins.SchShapeComp
{
	[Serializable]
	public class DynamicText : StaticText, IDynamicComponent
	{
		/// <summary>
		/// Текст надписи по умолчанию
		/// </summary>
		new public static readonly string DefaultText =
			Localization.UseRussian ? "Динамическая надпись" : "Dynamic text";


		/// <summary>
		/// Конструктор
		/// </summary>
		public DynamicText()
			: base()
		{
			Text = DefaultText;
			BackColorOnHover = "";
			BorderColorOnHover = "";
			ForeColorOnHover = "";
			UnderlineOnHover = false;
			Action = Actions.None;
			ShowValue = ShowValueKinds.ShowWithUnit;
			Conditions = new List<DynamicTextConditions>();
			InCnlNum = 0;
			CtrlCnlNum = 0;
			InCnlNumCustom = "(0) NA";
			CtrlCnlNumCustom = "(0) NA";
		}


		/// <summary>
		/// Получить или установить цвет фона при наведении указателя мыши
		/// </summary>
		#region Attributes
		[DisplayName("Back color on hover"), Category(Categories.Behavior)]
		[Description("The background color of the component when user rests the pointer on it.")]
		[CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		#endregion
		public string BackColorOnHover { get; set; }

		/// <summary>
		/// Получить или установить цвет рамки при наведении указателя мыши
		/// </summary>
		#region Attributes
		[DisplayName("Border color on hover"), Category(Categories.Behavior)]
		[Description("The border color of the component when user rests the pointer on it.")]
		[CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		#endregion
		public string BorderColorOnHover { get; set; }

		/// <summary>
		/// Получить или установить цвет текста при наведени указателя мыши
		/// </summary>
		#region Attributes
		[DisplayName("Fore color on hover"), Category(Categories.Behavior)]
		[Description("The foreground color of the component, which is used to display text, when user rests the pointer on it.")]
		[CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		#endregion
		public string ForeColorOnHover { get; set; }

		/// <summary>
		/// Получить или установить признак подчёркивания при наведении указателя мыши
		/// </summary>
		#region Attributes
		[DisplayName("Underline on hover"), Category(Categories.Behavior)]
		[Description("Underline text when user rests the pointer on the component.")]
		[CM.DefaultValue(false), CM.TypeConverter(typeof(BooleanConverter))]
		#endregion
		public bool UnderlineOnHover { get; set; }

		/// <summary>
		/// Получить или установить действие
		/// </summary>
		#region Attributes
		[DisplayName("Action"), Category(Categories.Behavior)]
		[Description("The action executed by clicking the left mouse button on the component.")]
		[CM.DefaultValue(Actions.None)]
		#endregion
		public Actions Action { get; set; }

		/// <summary>
		/// Получить или установить признак вывода значения входного канала
		/// </summary>
		#region Attributes
		[DisplayName("Show value"), Category(Categories.Behavior)]
		[Description("Show a value of the input channel associated with the component.")]
		[CM.DefaultValue(ShowValueKinds.ShowWithUnit)]
		#endregion
		public ShowValueKinds ShowValue { get; set; }

		/// <summary>
		/// Получить или установить номер входного канала
		/// </summary>
		#region Attributes
		[CM.Browsable(false)]
		[DisplayName("Input channel"), Category(Categories.Data)]
		[Description("The input channel number associated with the component.")]
		[CM.DefaultValue(0)]
		#endregion
		public int InCnlNum { get; set; }


		[DisplayName("Conditions"), Category(Categories.Behavior)]
		[Description("The conditions for SVG Shape output depending on the value of the input channel.")]
		[CM.DefaultValue(null), CM.TypeConverter(typeof(CollectionConverter))]
		[CM.Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
		public List<DynamicTextConditions> Conditions { get; protected set; }


		/// <summary>
		/// Получить или установить номер входного канала
		/// </summary>
		#region Attributes
		[DisplayName("Input channel"), Category(Categories.Data)]
		[Description("The input channel number associated with the component.")]
		[CM.Editor(typeof(IDcustomEditor), typeof(UITypeEditor))]
		#endregion
		public string InCnlNumCustom { get; set; }


		/// <summary>
		/// Получить или установить номер канала управления
		/// </summary>
		#region Attributes
		[DisplayName("Output channel"), Category(Categories.Data)]
		[Description("The output channel number associated with the component.")]
		[CM.Editor(typeof(IDcustomEditor), typeof(UITypeEditor))]
		#endregion
		public string CtrlCnlNumCustom { get; set; }

		/// <summary>
		/// Получить или установить номер канала управления
		/// </summary>
		#region Attributes
		[CM.Browsable(false)]
		[DisplayName("Output channel"), Category(Categories.Data)]
		[Description("The output channel number associated with the component.")]
		[CM.DefaultValue(0)]
		#endregion
		public int CtrlCnlNum { get; set; }



		[DisplayName("Rotation"), Category(Categories.Appearance)]
		[Description("The rotation angle of the SVG shape in degrees.")]
		[CM.DefaultValue(0)]
		public int Rotation { get; set; }

		/// <summary>
		/// Загрузить конфигурацию компонента из XML-узла
		/// </summary>
		public override void LoadFromXml(XmlNode xmlNode)
		{
			base.LoadFromXml(xmlNode);

			BackColorOnHover = xmlNode.GetChildAsString("BackColorOnHover");
			BorderColorOnHover = xmlNode.GetChildAsString("BorderColorOnHover");
			ForeColorOnHover = xmlNode.GetChildAsString("ForeColorOnHover");
			UnderlineOnHover = xmlNode.GetChildAsBool("UnderlineOnHover");
			Rotation = xmlNode.GetChildAsInt("Rotation");
			Action = xmlNode.GetChildAsEnum<Actions>("Action");
			ShowValue = xmlNode.GetChildAsEnum<ShowValueKinds>("ShowValue");
			InCnlNum = xmlNode.GetChildAsInt("InCnlNum");
			CtrlCnlNum = xmlNode.GetChildAsInt("CtrlCnlNum");
			InCnlNumCustom = xmlNode.GetChildAsString("InCnlNumCustom");
			CtrlCnlNumCustom = xmlNode.GetChildAsString("CtrlCnlNumCustom");

			XmlNode conditionsNode = xmlNode.SelectSingleNode("Conditions");

			if (conditionsNode != null)
			{
				Conditions = new List<DynamicTextConditions>();
				XmlNodeList conditionNodes = conditionsNode.SelectNodes("Condition");
				foreach (XmlNode conditionNode in conditionNodes)
				{
					DynamicTextConditions condition = new DynamicTextConditions { SchemeView = SchemeView };
					condition.LoadFromXml(conditionNode);
					Conditions.Add(condition);
				}
			}

		}

		/// <summary>
		/// Сохранить конфигурацию компонента в XML-узле
		/// </summary>
		public override void SaveToXml(XmlElement xmlElem)
		{
			base.SaveToXml(xmlElem);
			XmlElement conditionsElem = xmlElem.AppendElem("Conditions");
			foreach (DynamicTextConditions condition in Conditions)
			{
				XmlElement conditionElem = conditionsElem.AppendElem("Condition");
				condition.SaveToXml(conditionElem);
			}
			xmlElem.AppendElem("Rotation", Rotation);
			xmlElem.AppendElem("BackColorOnHover", BackColorOnHover);
			xmlElem.AppendElem("BorderColorOnHover", BorderColorOnHover);
			xmlElem.AppendElem("ForeColorOnHover", ForeColorOnHover);
			xmlElem.AppendElem("UnderlineOnHover", UnderlineOnHover);
			xmlElem.AppendElem("Action", Action);
			xmlElem.AppendElem("ShowValue", ShowValue);
            if(InCnlNumCustom != null && (InCnlNum == null || InCnlNum == 0) && FindNumberInInCnlNumCustom(InCnlNumCustom) > 0)
            {
                InCnlNum = FindNumberInInCnlNumCustom(InCnlNumCustom);
            }
            xmlElem.AppendElem("InCnlNum", InCnlNum);
            xmlElem.AppendElem("CtrlCnlNum", CtrlCnlNum);
            xmlElem.AppendElem("CtrlCnlNumCustom", CtrlCnlNumCustom);
            xmlElem.AppendElem("InCnlNumCustom", InCnlNumCustom);
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
    }

}