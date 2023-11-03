using System;
using System.ComponentModel;
using System.Globalization;


namespace Scada.Web.Plugins.SchShapeComp.PropertyGrid
{
	public class SVGTypeConverter : TypeConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				string stringValue = value as string;
				if (!string.IsNullOrEmpty(stringValue))
				{
					return stringValue.Length > 5 ? $"{stringValue.Substring(0, 20)}..." : stringValue;
				}
				else
				{
					return "Import SVG file";
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}

}