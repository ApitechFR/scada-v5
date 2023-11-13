using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Scada.Web.Plugins.SchShapeComp.PropertyGrid
{
	public class SVGEditor : UITypeEditor
	{
		private static bool isCustomShapeFormOpen = false;
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

			if (editorService != null)
			{
				if (isCustomShapeFormOpen)
				{
					MessageBox.Show("The Custom Shape editor is already open.");
					return value;
				}
				string currentSvg = value as string;

				FrmCustomShape frmCustomShape = new FrmCustomShape(currentSvg);

				frmCustomShape.ShapeSaved += (svgData) =>
				{

					value = svgData;
					context.PropertyDescriptor.SetValue(context.Instance, svgData);
				};
				frmCustomShape.FormClosed += (sender, e) =>
				{
					isCustomShapeFormOpen = false;
				};

				isCustomShapeFormOpen = true;

				frmCustomShape.Show();
				return value;
			}
			return value;
		}

	}
}