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
	/// NumberSelectEditor class: a custom editor for PropertyGrid
	/// that allows you to select a number from a dropdown list.
	/// </summary>
	public class NumberSelectEditor : UITypeEditor
	{
		/// <summary>
		/// Determine the editing style used by the editor (DropDown style).
		/// </summary>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		/// <summary>
		/// Edits the value of the specified object using the editor style indicated by the GetEditStyle method.
		/// ListBox with numbers from 3 to 10, excluding 7 and 9.
		/// </summary>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

			if (editorService != null)
			{
				ListBox listBox = new ListBox();
				for (int i = 3; i <= 10; i++)
				{
					if(!i.Equals(7) && !i.Equals(9))
						listBox.Items.Add(i);
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
