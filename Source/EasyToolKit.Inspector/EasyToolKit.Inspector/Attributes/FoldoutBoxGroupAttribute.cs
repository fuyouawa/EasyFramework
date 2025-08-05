using System;
using System.Diagnostics;
using EasyToolKit.Inspector;

[assembly: RegisterGroupAttributeScope(typeof(FoldoutBoxGroupAttribute), typeof(EndFoldoutBoxGroupAttribute))]

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class FoldoutBoxGroupAttribute : BeginGroupAttribute
    {
        public string Label;
        public bool? Expanded;

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
