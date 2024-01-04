using Scada.Scheme;
using Scada.Web.Properties;
using System.Collections.Generic;


namespace Scada.Web.Plugins.SchShapeComp
{
	public class ShapeCompLibSpec : CompLibSpec
	{
		public override string XmlPrefix => "shape";
		public override string XmlNs => "urn:rapidscada:scheme:shape";
		public override string GroupHeader => "Shape";

		public override List<string> Styles
		{
			get
			{
				return new List<string>()
				{
					"SchShapeComp/css/shapecomp.min.css"
				};
			}
		}
		public override List<string> Scripts
		{
			get
			{
				return new List<string>()
				{
					"SchShapeComp/js/shapecomprender.js"
				};
			}
		}

		protected override List<CompItem> CreateCompItems()
		{
			return new List<CompItem>()
			{
				new CompItem(Resources.shape,typeof(BasicShape)),
				new CompItem(Resources.svg,	typeof(CustomSVG)),
				new CompItem(Resources.barre,typeof(BarGraph)),
				new CompItem(Resources.comp_dp,typeof(DynamicPicture)),
				new CompItem(Resources.comp_dt,typeof(DynamicText)),
			};
		}
		protected override CompFactory CreateCompFactory()
		{
			return new ShapeCompFactory();
		}
	}

}