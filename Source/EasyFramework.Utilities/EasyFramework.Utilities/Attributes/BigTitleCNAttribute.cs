using System;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.Utilities
{
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public class BigTitleCNAttribute : Attribute
    {
        /// <summary>
        /// The title displayed above the property in the inspector.
        /// </summary>
        public string Title;

        /// <summary>
        /// Optional subtitle.
        /// </summary>
        public string Subtitle;

        /// <summary>
        /// If <c>true</c> the title will be displayed with a bold font.
        /// </summary>
        public bool Bold;

        /// <summary>
        /// Gets a value indicating whether or not to draw a horizontal line below the title.
        /// </summary>
        public bool HorizontalLine;

        /// <summary>
        /// Title alignment.
        /// </summary>
        public TitleAlignments TitleAlignment;

        /// <summary>
        /// Creates a title above any property in the inspector.
        /// </summary>
        /// <param name="title">The title displayed above the property in the inspector.</param>
        /// <param name="subtitle">Optional subtitle</param>
        /// <param name="titleAlignment">Title alignment</param>
        /// <param name="horizontalLine">Horizontal line</param>
        /// <param name="bold">If <c>true</c> the title will be drawn with a bold font.</param>
        public BigTitleCNAttribute(string title, string subtitle = null, TitleAlignments titleAlignment = TitleAlignments.Left, bool horizontalLine = true, bool bold = true)
        {
            Title = title ?? "null";
            Subtitle = subtitle;
            Bold = bold;
            TitleAlignment = titleAlignment;
            HorizontalLine = horizontalLine;
        }
    }
}
