using System;

namespace Scada.Scheme.Model
{

    [Serializable]
    public class Alias
    {
        public string Name { get; set; }
        public Type AliasType { get; set; }
        public bool isCnlLinked { get; set; }

        private object _value;
        public object Value { get { return _value; } set { 
                if (value.GetType() == AliasType)
                {
                    _value = value;
                }
            } }
        public Alias()
        {
            Name = "";
            AliasType = typeof(string);
            isCnlLinked = false;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
