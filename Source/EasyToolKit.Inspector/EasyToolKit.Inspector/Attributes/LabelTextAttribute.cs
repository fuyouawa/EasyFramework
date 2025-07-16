using System;

namespace EasyToolKit.Inspector
{
    public class LabelTextAttribute : Attribute
    {
        public string Label;

        public LabelTextAttribute(string label)
        {
            Label = label;
        }
    }
}
