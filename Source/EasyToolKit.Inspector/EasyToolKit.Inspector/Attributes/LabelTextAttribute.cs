using System;

namespace EasyToolKit.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class LabelTextAttribute : Attribute
    {
        public string Label;

        public LabelTextAttribute(string label)
        {
            Label = label;
        }
    }
}
