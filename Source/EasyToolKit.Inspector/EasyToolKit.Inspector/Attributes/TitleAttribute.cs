using System;
using System.Diagnostics;
using UnityEngine;

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class TitleAttribute : Attribute
    {
        public string Title;
        public bool BoldTitle = true;
        public string Subtitle;

        public bool HorizontalLine = true;
        public TextAlignment TextAlignment = TextAlignment.Left;

        public TitleAttribute(string title)
        {
            Title = title;
        }
    }
}