using System;
using System.Diagnostics;
using EasyToolKit.Inspector;

[assembly: RegisterGroupAttributeScope(typeof(FoldoutBoxGroupAttribute), typeof(EndFoldoutBoxGroupAttribute))]

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class FoldoutBoxGroupAttribute : BeginGroupAttribute
    {
        public string Label { get; set; }
        public bool? Expanded { get; set; }

        public FoldoutBoxGroupAttribute(string label)
        {
            Label = label;
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class EndFoldoutBoxGroupAttribute : EndGroupAttribute
    {
    }
}
