using EasyToolKit.Inspector;
using System.Diagnostics;
using UnityEngine;

[assembly: RegisterGroupAttributeScope(typeof(AwesomeFoldoutGroupAttribute), typeof(EndAwesomeFoldoutGroupAttribute))]

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class AwesomeFoldoutGroupAttribute: BeginGroupAttribute
    {
        public string Label;
        public Color SideLineColor = Color.green;
        public string IconTextureGetter;
        public bool? Expanded;

        public AwesomeFoldoutGroupAttribute(string label)
        {
            Label = label;
        }
    }
    
    [Conditional("UNITY_EDITOR")]
    public class EndAwesomeFoldoutGroupAttribute : EndGroupAttribute
    {
        public EndAwesomeFoldoutGroupAttribute()
        {
        }
    }
}
