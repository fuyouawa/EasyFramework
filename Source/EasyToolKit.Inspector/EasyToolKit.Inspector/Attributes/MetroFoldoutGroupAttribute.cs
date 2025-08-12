using System;
using EasyToolKit.Inspector;
using System.Diagnostics;
using UnityEngine;

[assembly: RegisterGroupAttributeScope(typeof(MetroFoldoutGroupAttribute), typeof(EndMetroFoldoutGroupAttribute))]

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class MetroFoldoutGroupAttribute : BeginGroupAttribute
    {
        public string Label { get; set; }
        public Color SideLineColor { get; set; } = Color.green;
        public string IconTextureGetter { get; set; }

        private bool? _expanded;

        public bool Expanded
        {
            get => _expanded ?? throw new InvalidOperationException();
            set => _expanded = value;
        }

        public bool IsDefinedExpanded => _expanded.HasValue;

        public MetroFoldoutGroupAttribute(string label)
        {
            Label = label;
        }
    }

    [Conditional("UNITY_EDITOR")]
    public class EndMetroFoldoutGroupAttribute : EndGroupAttribute
    {
        public EndMetroFoldoutGroupAttribute()
        {
        }
    }
}
