using Scada.Scheme.Model.PropertyGrid;
using Scada.Web.Plugins.SchShapeComp.PropertyGrid;
using System;
using System.Drawing.Design;
using CM = System.ComponentModel;


namespace Scada.Web.SchShapeComp.PropertyGrid
{
	[Serializable]
	public class CustomSVGCondition: AdvancedCondition
	{
		public CustomSVGCondition()
			: base()
		{
			TextContent = "";
			IsVisible = true;
			Blinking = BlinkingSpeed.None;
		}

		#region Attributes
		[CM.Browsable(false)]
		#endregion
		public int Width { get; set; }

		[CM.Browsable(false)]
		[DisplayName("Height"), Category(Categories.Appearance)]
		public int Height { get; set; }

		[CM.Browsable(false)]
		[DisplayName("Background Color"), Category(Categories.Appearance)]
		[CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
		public string BackgroundColor { get; set; }
	}
}