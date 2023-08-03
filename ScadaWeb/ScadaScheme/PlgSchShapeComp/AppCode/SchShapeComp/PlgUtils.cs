using System.Reflection;

namespace Scada.Web.Plugins.SchShapeComp
{
	/// <summary>
	/// Controls class loading when cloning components
	/// </summary>
	internal static class PlgUtils
	{

		public static readonly SerializationBinder SerializationBinder =
			new SerializationBinder(Assembly.GetExecutingAssembly());
	}

}