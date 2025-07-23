using System;
using System.Diagnostics;

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class LabelTextAttribute : Attribute
    {
        public string Label;

        public LabelTextAttribute(string label)
        {
            Label = label;
        }
    }
}
