using System;
using System.Diagnostics;
using UnityEngine;

namespace EasyToolKit.Inspector
{
    [Conditional("UNITY_EDITOR")]
    public class TitleAttribute : Attribute
    {
        public string Title { get; set; }
        public bool BoldTitle { get; set; } = true;
        public string Subtitle { get; set; }

        public bool HorizontalLine { get; set; } = true;
        public TextAlignment TextAlignment { get; set; } = TextAlignment.Left;

        public TitleAttribute(string title)
        {
            Title = title;
        }
    }
}