using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

using ListBox = System.Windows.Forms.ListBox;

namespace Scada.Web.Plugins.SchShapeComp.PropertyGrid
{
	public class BasicShapeSelectEditor : UITypeEditor
	{

		/// <param name="context"></param>
		/// <returns></returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

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
				string[] shapes = { "Circle",  "Line", "Rectangle" };

				foreach (string shape in shapes)
				{
					listBox.Items.Add(shape);
				}

				listBox.SelectedIndexChanged += delegate (object sender, EventArgs e)
				{
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