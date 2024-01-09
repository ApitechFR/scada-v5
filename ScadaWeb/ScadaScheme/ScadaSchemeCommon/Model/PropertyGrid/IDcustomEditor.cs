using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Reflection;

namespace Scada.Scheme.Model.PropertyGrid
{
    public class IDcustomEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorSvc = provider == null ? null :
                (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

			object _instance = GetInstanceFromContext(context);

			if (context != null && context.Instance != null && editorSvc != null)
            {
                FrmIDcustomDialog form = new FrmIDcustomDialog();
                
                if(editorSvc.ShowDialog(form) == DialogResult.OK)
                {
                    value = form.getValue();
                    if(_instance is IDynamicComponent instance)
                    {
                        if (context.ToString().Contains("Input"))
                        {
                            instance.InCnlNum = form.getNumValue();
                        }
                        else if (context.ToString().Contains("Output"))
                            instance.CtrlCnlNum = form.getNumValue();
                    }
                }
            }

            return value;
        }
		private object GetInstanceFromContext(ITypeDescriptorContext context)
		{
			if (context?.Instance == null)
				return null;

			try
			{
				Type type = context.Instance.GetType();
				FieldInfo baseTypeDescriptorField = type.GetField("baseTypeDescriptor", BindingFlags.NonPublic | BindingFlags.Instance);

				if (baseTypeDescriptorField == null)
					return null;

				object baseTypeDescriptor = baseTypeDescriptorField.GetValue(context.Instance);
				if (baseTypeDescriptor == null)
					return null;

				Type baseType = baseTypeDescriptor.GetType();
				FieldInfo fieldInfo = baseType.GetField("_instance", BindingFlags.NonPublic | BindingFlags.Instance);

				if (fieldInfo == null)
					return null;

				return fieldInfo.GetValue(baseTypeDescriptor);
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
