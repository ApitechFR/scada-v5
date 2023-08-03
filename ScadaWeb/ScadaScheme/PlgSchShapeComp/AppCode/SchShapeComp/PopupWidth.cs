using Scada.Scheme.Model.PropertyGrid;
using CM = System.ComponentModel;

namespace Scada.Web.Plugins.SchShapeComp
{
	[CM.TypeConverter(typeof(EnumConverter))]
	public enum PopupWidth
	{
		/// <summary>
		/// Normal
		/// </summary>
		#region Attributes
		[Description("Normal")]
		#endregion
		Normal,

		/// <summary>
		/// Small
		/// </summary>
		#region Attributes
		[Description("Small")]
		#endregion
		Small,

		/// <summary>
		/// Large
		/// </summary>
		#region Attributes
		[Description("Large")]
		#endregion
		Large
	}
}