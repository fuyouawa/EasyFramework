using System;

namespace EasyToolKit.Inspector
{
    public class ShowIfAttribute : Attribute
    {
        public string Condition { get; set; }
        public object Value { get; set; }

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
