using Scada.Scheme;
using Scada.Web.Properties;
using System.Collections.Generic;


namespace Scada.Web.Plugins.SchSvgComp
{
	public class SvgCompLibSpec : CompLibSpec
	{
		public override string XmlPrefix => "svg";
		public override string XmlNs => "urn:rapidscada:scheme:svg";
		public override string GroupHeader => "Svg Shape";

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
				new CompItem(Resources.svg,typeof(SvgShape))
			};
		}
		protected override CompFactory CreateCompFactory()
		{
			return new SvgCompFactory();
		}
	}

}