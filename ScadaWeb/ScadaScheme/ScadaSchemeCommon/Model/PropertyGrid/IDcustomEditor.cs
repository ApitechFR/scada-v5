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
    internal class IDcustomEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorSvc = provider == null ? null :
                (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (context != null && context.Instance != null && editorSvc != null)
            {
                FrmIDcustomDialog form = new FrmIDcustomDialog();

                if(editorSvc.ShowDialog(form) == DialogResult.OK)
                {
                   // value = form.
                }
                // font = value as Font;
                //FrmFontDialog frmFontDialog = new FrmFontDialog(font);

                //if (editorSvc.ShowDialog(frmFontDialog) == DialogResult.OK)
                //    value = frmFontDialog.FontResult;
            }

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
