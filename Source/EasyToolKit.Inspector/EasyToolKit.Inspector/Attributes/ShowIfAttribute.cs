using System;

namespace EasyToolKit.Inspector
{
    public class ShowIfAttribute : Attribute
    {
        public string Condition;
        public object Value;

        public ShowIfAttribute(string condition)
        {
            Condition = condition;
            Value = true;
        }

        public ShowIfAttribute(string condition, object value)
        {
            Condition = condition;
            Value = value;
        }
    }
}
