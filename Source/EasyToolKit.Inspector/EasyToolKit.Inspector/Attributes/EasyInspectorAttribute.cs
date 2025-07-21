using System;
using System.Diagnostics;

namespace EasyToolKit.Inspector
{
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("UNITY_EDITOR")]
    public class EasyInspectorAttribute : Attribute
    {
    }
}
