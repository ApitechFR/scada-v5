using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Xml;
using CM = System.ComponentModel;
namespace Scada.Scheme.Model
{
	public class DynamicPolygon : StaticPolygon, IDynamicComponent
	{
		public DynamicPolygon()
            : base()
        {
            BackColorOnHover = "";
            BorderColorOnHover = "";
            Action = Actions.None;
            Conditions = new List<ImageCondition>();
            InCnlNum = 0;
            CtrlCnlNum = 0;
            InCnlNumCustom = "0";
            CtrlCnlNumCustom = "0";
        }

        [DisplayName("Back color on hover"), Category(Categories.Behavior)]
        [Description("The background color of the component when user rests the pointer on it.")]
        [CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        public string BackColorOnHover { get; set; }

        [DisplayName("Border color on hover"), Category(Categories.Behavior)]
        [Description("The border color of the component when user rests the pointer on it.")]
        [CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        public string BorderColorOnHover { get; set; }

        [DisplayName("Action"), Category(Categories.Behavior)]
        [Description("The action executed by clicking the left mouse button on the component.")]
        [CM.DefaultValue(Actions.None)]
        public Actions Action { get; set; }

        [DisplayName("Conditions"), Category(Categories.Behavior)]
        [Description("The conditions for image output depending on the value of the input channel.")]
        [CM.DefaultValue(null), CM.TypeConverter(typeof(CollectionConverter))]
        [CM.Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        public List<ImageCondition> Conditions { get; protected set; }

        [CM.Browsable(false)]
        [DisplayName("Input channel"), Category(Categories.Data)]
        [Description("The input channel number associated with the component.")]
        [CM.DefaultValue(0)]
        public int InCnlNum { get; set; }

        [DisplayName("Input channel"), Category(Categories.Data)]
        [Description("The input channel number associated with the component.")]
        [CM.Editor(typeof(IDcustomEditor), typeof(UITypeEditor))]
        public string InCnlNumCustom { get; set; }

        [CM.Browsable(false)]
        [DisplayName("Output channel"), Category(Categories.Data)]
        [Description("The output channel number associated with the component.")]
        [CM.DefaultValue(0)]
        public int CtrlCnlNum { get; set; }

        [DisplayName("Output channel"), Category(Categories.Data)]
        [Description("The output channel number associated with the component.")]
        [CM.Editor(typeof(IDcustomEditor), typeof(UITypeEditor))]
        public string CtrlCnlNumCustom { get; set; }

        public override void LoadFromXml(XmlNode xmlNode)
        {
            base.LoadFromXml(xmlNode);

            BackColorOnHover = xmlNode.GetChildAsString("BackColorOnHover");
            BorderColorOnHover = xmlNode.GetChildAsString("BorderColorOnHover");
            Action = xmlNode.GetChildAsEnum<Actions>("Action");

            XmlNode conditionsNode = xmlNode.SelectSingleNode("Conditions");
            if (conditionsNode != null)
            {
                Conditions = new List<ImageCondition>();
                XmlNodeList conditionNodes = conditionsNode.SelectNodes("Condition");
                foreach (XmlNode conditionNode in conditionNodes)
                {
                    ImageCondition condition = new ImageCondition { SchemeView = SchemeView };
                    condition.LoadFromXml(conditionNode);
                    Conditions.Add(condition);
                }
            }

            InCnlNum = xmlNode.GetChildAsInt("InCnlNum");
            CtrlCnlNum = xmlNode.GetChildAsInt("CtrlCnlNum");
            InCnlNumCustom = xmlNode.GetChildAsString("InCnlNumCustom");
            CtrlCnlNumCustom = xmlNode.GetChildAsString("CtrlCnlNumCustom");
        }

        public override void SaveToXml(XmlElement xmlElem)
        {
            base.SaveToXml(xmlElem);

            xmlElem.AppendElem("BackColorOnHover", BackColorOnHover);
            xmlElem.AppendElem("BorderColorOnHover", BorderColorOnHover);
            xmlElem.AppendElem("Action", Action.ToString());

            XmlElement conditionsElem = xmlElem.AppendElem("Conditions");
            foreach (ImageCondition condition in Conditions)
            {
                XmlElement conditionElem = conditionsElem.AppendElem("Condition");
                condition.SaveToXml(conditionElem);
            }

            xmlElem.AppendElem("InCnlNum", InCnlNum);
            xmlElem.AppendElem("CtrlCnlNum", CtrlCnlNum);
            xmlElem.AppendElem("InCnlNumCustom", InCnlNumCustom);
            xmlElem.AppendElem("CtrlCnlNumCustom", CtrlCnlNumCustom);
        }
	}
}
