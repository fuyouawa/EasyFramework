using System;

namespace EasyToolKit.Inspector
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ButtonAttribute : Attribute
    {
        public string Label { get; set; }

        public ButtonAttribute()
        {
            Label = null;
        }

        public ButtonAttribute(string label)
        {
            Label = label;
        }
    }
}