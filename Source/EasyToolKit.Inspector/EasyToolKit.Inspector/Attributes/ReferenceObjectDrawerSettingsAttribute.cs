using System;
using System.Diagnostics;

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class ReferenceObjectDrawerSettingsAttribute : Attribute
    {
        public bool HideFoldout { get; set; }
        public bool HideIfNull { get; set; }
        public bool? InstantiateIfNull { get; set; }

        public ReferenceObjectDrawerSettingsAttribute()
        {
        }
    }
}