using Scada.Scheme.Model.PropertyGrid;
using CM = System.ComponentModel;

namespace Scada.Web.Plugins.SchBasicComp
{
	/// <summary>
	/// Component properties that can be bound to an input channel
	/// <para>Properties of the component that can be bound to the input channel</para>
	/// </summary>
	[CM.TypeConverter(typeof(EnumConverter))]
	public enum BoundProperties
	{
		/// <summary>
		/// Not set
		/// </summary>
		#region Attributes
		[Description("None")]
		#endregion
		None,

		/// <summary>
		/// Availability
		/// </summary>
		#region Attributes
		[Description("Enabled")]
		#endregion
		Enabled,

		/// <summary>
		/// Visibility
		/// </summary>
		#region Attributes
		[Description("Visible")]
		#endregion
		Visible
	}
}
