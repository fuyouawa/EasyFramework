using System;
using System.Diagnostics;
using Sirenix.OdinInspector;

namespace EasyFramework.ToolKit
{
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public class TitleExAttribute : Attribute
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
        public bool BoldTitle;

        /// <summary>
        /// Gets a value indicating whether or not to draw a horizontal line below the title.
        /// </summary>
        public bool HorizontalLine;

        /// <summary>
        /// Title alignment.
        /// </summary>
        public TitleAlignments TitleAlignment;

        public int TitleFontSize;

        /// <summary>
        /// Creates a title above any property in the inspector.
        /// </summary>
        /// <param name="title">The title displayed above the property in the inspector.</param>
        /// <param name="subtitle">Optional subtitle</param>
        public TitleExAttribute(string title, string subtitle = null)
        {
            Title = title ?? "null";
            Subtitle = subtitle;

            BoldTitle = true;
            TitleAlignment = TitleAlignments.Left;
            HorizontalLine = true;
            TitleFontSize = 13;
        }
    }
}
