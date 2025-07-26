using System;
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

        private bool? _expanded;

        public bool Expanded
        {
            get => _expanded ?? throw new InvalidOperationException();
            set => _expanded = value;
        }

        public bool IsDefinedExpanded => _expanded.HasValue;

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
