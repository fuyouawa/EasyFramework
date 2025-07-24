using System;

namespace EasyToolKit.Inspector
{
    public class HideIfAttribute : Attribute
    {
        public string Condition;
        public object Value;

        public HideIfAttribute(string condition)
        {
            Condition = condition;
            Value = true;
        }

        public HideIfAttribute(string condition, object value)
        {
            Condition = condition;
            Value = value;
        }
    }
}
