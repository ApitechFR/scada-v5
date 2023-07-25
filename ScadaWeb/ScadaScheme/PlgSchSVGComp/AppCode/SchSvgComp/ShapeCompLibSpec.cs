using Scada.Scheme;
using Scada.Web.Properties;
using System.Collections.Generic;


namespace Scada.Web.Plugins.SchSvgComp
{
	public class ShapeCompLibSpec : CompLibSpec
	{
		public override string XmlPrefix => "svg";
		public override string XmlNs => "urn:rapidscada:scheme:svg";
		public override string GroupHeader => "Shape";

		public override List<string> Styles
		{
			get
			{
				return new List<string>()
				{
					"SchSvgComp/css/svgcomp.min.css"
				};
			}
		}
		public override List<string> Scripts
		{
			get
			{
				return new List<string>()
				{
					"SchSvgComp/js/svgcomprender.js"
				};
			}
		}

		protected override List<CompItem> CreateCompItems()
		{
			return new List<CompItem>()
			{
				new CompItem(Resources.svg,typeof(SvgShape)),
				new CompItem(Resources.polygon, typeof(Polygon)),
				new CompItem(Resources.svg,	typeof(CustomSVG)),
			};
		}
		protected override CompFactory CreateCompFactory()
		{
			return new ShapeCompFactory();
		}
	}

}