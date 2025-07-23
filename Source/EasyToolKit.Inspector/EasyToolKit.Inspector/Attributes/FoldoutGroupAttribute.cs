using System;
using System.Diagnostics;
using EasyToolKit.Inspector;

[assembly: RegisterGroupAttributeScope(typeof(FoldoutGroupAttribute), typeof(EndFoldoutGroupAttribute))]

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class FoldoutGroupAttribute : BeginGroupAttribute
    {
        public string Label;
        public bool? Expanded;

        public FoldoutGroupAttribute(string label)
        {
            Label = label;
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class EndFoldoutGroupAttribute : EndGroupAttribute
    {
        public bool IncludeCurrent;
    }
}
