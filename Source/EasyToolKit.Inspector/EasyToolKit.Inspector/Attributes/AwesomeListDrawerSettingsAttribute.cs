using System;
using System.Diagnostics;
using UnityEngine;

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class AwesomeListDrawerSettingsAttribute : ListDrawerSettingsAttribute
    {
        public Color SideLineColor = Color.green;
        public string IconTextureGetter;

        public AwesomeListDrawerSettingsAttribute()
        {
        }
    }
}
