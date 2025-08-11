using System;
using System.Diagnostics;

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class ReferenceObjectDrawerSettingsAttribute : Attribute
    {
        public bool HideFoldout;
        public bool HideIfNull;
        public bool? InstantiateIfNull;

        public ReferenceObjectDrawerSettingsAttribute()
        {
        }
    }
}