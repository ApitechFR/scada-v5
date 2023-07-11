using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace Scada.Scheme.Model.PropertyGrid
{
	/// <summary>
	/// 
	/// </summary>
	public class SvgShapeSelectEditor : UITypeEditor
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

			if (editorService != null)
			{
				ListBox listBox = new ListBox();
				string[] svgShapes = { "Polygon", "Triangle", "Rectangle", "Circle", "Line", "Polyline", "Custom ..." };

				foreach (string shape in svgShapes)
				{
					listBox.Items.Add(shape);
				}

				listBox.SelectedIndexChanged += delegate (object sender, EventArgs e)
				{
					if (listBox.SelectedItem.ToString().Contains( "Custom"))
					{
						FrmCustomShape frmCustomShape = new FrmCustomShape();
						frmCustomShape.ShowDialog();

					}
					editorService.CloseDropDown();
				};

				editorService.DropDownControl(listBox);
				if (listBox.SelectedItem != null)
				{
					return listBox.SelectedItem;
				}
			}

			return value;
		}
	}

}
