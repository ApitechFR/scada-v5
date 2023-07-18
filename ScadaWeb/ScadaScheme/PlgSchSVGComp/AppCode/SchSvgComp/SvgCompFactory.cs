using Scada.Scheme;
using Scada.Scheme.Model;


namespace Scada.Web.Plugins.SchSvgComp
{
	public class SvgCompFactory : CompFactory
	{
		public override BaseComponent CreateComponent(string typeName, bool nameIsShort)
		{
			if (NameEquals("SvgShape", typeof(SvgShape).FullName, typeName, nameIsShort))
				return new SvgShape();
			else
				return null;
        }
	}
}