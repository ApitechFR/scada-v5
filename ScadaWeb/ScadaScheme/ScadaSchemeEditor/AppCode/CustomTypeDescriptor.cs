using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Scada.Scheme.Editor
{
    public class CustomTypeDescriptor : ICustomTypeDescriptor
    {
        private ICustomTypeDescriptor baseTypeDescriptor;
        private PropertyDescriptorCollection properties;

        public CustomTypeDescriptor(ICustomTypeDescriptor baseTypeDescriptor, PropertyDescriptorCollection properties)
        {
            this.baseTypeDescriptor = baseTypeDescriptor;
            this.properties = properties;
        }

        public AttributeCollection GetAttributes()
        {
            return baseTypeDescriptor.GetAttributes();
        }

        public string GetClassName()
        {
            return baseTypeDescriptor.GetClassName();
        }

        public string GetComponentName()
        {
            return baseTypeDescriptor.GetComponentName();
        }

        public TypeConverter GetConverter()
        {
            return baseTypeDescriptor.GetConverter();
        }

        public EventDescriptor GetDefaultEvent()
        {
            return baseTypeDescriptor.GetDefaultEvent();
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return baseTypeDescriptor.GetDefaultProperty();
        }

        public object GetEditor(Type editorBaseType)
        {
            return baseTypeDescriptor.GetEditor(editorBaseType);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return baseTypeDescriptor.GetEvents(attributes);
        }

        public EventDescriptorCollection GetEvents()
        {
            return baseTypeDescriptor.GetEvents();
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return properties;
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return properties;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return baseTypeDescriptor.GetPropertyOwner(pd);
        }
    }
}
