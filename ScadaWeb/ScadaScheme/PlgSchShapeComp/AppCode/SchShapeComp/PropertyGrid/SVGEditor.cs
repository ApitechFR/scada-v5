using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Scada.Web.Plugins.SchShapeComp.PropertyGrid
{
	public class SVGEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

			if (editorService != null)
			{
				// Pass the current SVG to FrmCustomShape
				string currentSvg = value as string;
				FrmCustomShape frmCustomShape;
				if (currentSvg is string)
				{
					frmCustomShape = new FrmCustomShape(currentSvg);
				}
				else
				{
					frmCustomShape = new FrmCustomShape();
				}


				DialogResult dialogResult = frmCustomShape.ShowDialog();

				if (dialogResult == DialogResult.OK)
				{
					// Get the SVG from FrmCustomShape
					return frmCustomShape.ShapeType;
				}
			}

			return value;
		}
	}


}