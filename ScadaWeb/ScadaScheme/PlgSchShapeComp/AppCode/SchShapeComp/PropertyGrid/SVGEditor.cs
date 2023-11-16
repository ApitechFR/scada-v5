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
				
				Form mainForm = Application.OpenForms[0]; 
				if (mainForm != null)
				{
					mainForm.WindowState = FormWindowState.Minimized;
				}

				string currentSvg = value as string;
				FrmCustomShape frmCustomShape = currentSvg is string ? new FrmCustomShape(currentSvg) : new FrmCustomShape();
				frmCustomShape.TopMost = true;
				DialogResult dialogResult = frmCustomShape.ShowDialog();

				if (mainForm != null)
				{
					mainForm.WindowState = FormWindowState.Normal;
				}

				if (dialogResult == DialogResult.OK)
				{
					return frmCustomShape.ShapeType;
				}
			}

			return value;
		}

	}
}