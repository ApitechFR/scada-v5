using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Scada.Scheme.Editor.AppCode
{
    public class CustomPropertyDescriptor : PropertyDescriptor
    {
        private PropertyDescriptor basePropertyDescriptor;
        private string displayName;
        private bool isReadOnly;

        public CustomPropertyDescriptor(PropertyDescriptor basePropertyDescriptor, string displayName, bool isReadOnly)
            : base(basePropertyDescriptor)
        {
            this.basePropertyDescriptor = basePropertyDescriptor;
            this.displayName = displayName;
            this.isReadOnly = isReadOnly;
        }

        public override string DisplayName
        {
            get { return displayName; }
        }

        public override Type ComponentType => basePropertyDescriptor.ComponentType;

        public override bool IsReadOnly
        {
            get { return isReadOnly; }
        }

        public override Type PropertyType => basePropertyDescriptor.PropertyType;

        public override bool CanResetValue(object component) => basePropertyDescriptor.CanResetValue(component);

        public override object GetValue(object component) => basePropertyDescriptor.GetValue(component);

        public override void ResetValue(object component) => basePropertyDescriptor.ResetValue(component);

        public override void SetValue(object component, object value) => basePropertyDescriptor.SetValue(component, value);

        public override bool ShouldSerializeValue(object component) => basePropertyDescriptor.ShouldSerializeValue(component);
    }
}
