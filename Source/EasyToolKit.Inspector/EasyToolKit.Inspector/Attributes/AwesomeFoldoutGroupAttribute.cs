using EasyToolKit.Inspector;
using UnityEngine;

[assembly: RegisterGroupAttributeScope(typeof(AwesomeFoldoutGroupAttribute), typeof(AwesomeEndFoldoutGroupAttribute))]

namespace EasyToolKit.Inspector
{
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

    public class AwesomeEndFoldoutGroupAttribute : EndGroupAttribute
    {
        public AwesomeEndFoldoutGroupAttribute()
        {
        }
    }
}
