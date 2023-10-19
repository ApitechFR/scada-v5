/*
 * Copyright 2019 Mikhail Shiryaev
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * 
 * Product  : Rapid SCADA
 * Module   : ScadaSchemeCommon
 * Summary  : Collection editor for PropertyGrid
 * 
 * Author   : Mikhail Shiryaev
 * Created  : 2019
 * Modified : 2019
 */

#pragma warning disable 1591 // CS1591: Missing XML comment for publicly visible type or member

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Utils;

namespace Scada.Scheme.Model.PropertyGrid
{
    /// <summary>
    /// Collection editor for PropertyGrid.
    /// <para>Редактор коллекции для PropertyGrid.</para>
    /// </summary>
    public class CollectionEditor : UITypeEditor
    {

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			object _instance = GetInstanceFromContext(context);

			if (_instance is BaseComponent component &&
				provider?.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService editorService &&
				value is IList list && value.GetType() is Type valueType && valueType.IsGenericType)
			{
				Type itemType = valueType.GetGenericArguments()[0];
				if (editorService.ShowDialog(new FrmCollectionDialog(list, itemType, component)) == DialogResult.OK)
					component.OnItemChanged(SchemeChangeTypes.ComponentChanged, component);
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
