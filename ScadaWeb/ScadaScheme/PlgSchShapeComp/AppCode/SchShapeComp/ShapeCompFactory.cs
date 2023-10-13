using Scada.Scheme;
using Scada.Scheme.Model;


namespace Scada.Web.Plugins.SchShapeComp
{
	public class ShapeCompFactory : CompFactory
	{
		public override BaseComponent CreateComponent(string typeName, bool nameIsShort)
		{
			if (NameEquals("BasicShape", typeof(BasicShape).FullName, typeName, nameIsShort))
				return new BasicShape();
			else if(NameEquals("CustomSVG", typeof(CustomSVG).FullName,typeName,nameIsShort)) 
				return new CustomSVG();
			else if(NameEquals("BarGraph", typeof(BarGraph).FullName, typeName, nameIsShort))
				return new BarGraph();
			else
				return null;
        }
	}
}