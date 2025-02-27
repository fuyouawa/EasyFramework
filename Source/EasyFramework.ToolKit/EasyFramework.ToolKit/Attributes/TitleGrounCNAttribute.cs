using Sirenix.OdinInspector;
using System;
using System.Diagnostics;

namespace EasyFramework.ToolKit
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class TitleGroupExAttribute : PropertyGroupAttribute
    {
        /// <summary>
        /// Optional subtitle.
        /// </summary>
        public string Subtitle;

        /// <summary>
        /// Title alignment.
        /// </summary>
        public TitleAlignments TitleAlignment;

        /// <summary>
        /// Gets a value indicating whether or not to draw a horizontal line below the title.
        /// </summary>
        public bool HorizontalLine;

        /// <summary>
        /// If <c>true</c> the title will be displayed with a bold font.
        /// </summary>
        public bool BoldTitle;

        /// <summary>
        /// Gets a value indicating whether or not to indent all group members.
        /// </summary>
        public bool Indent;

        /// <summary>
        /// Groups properties vertically together with a title, an optional subtitle, and an optional horizontal line. 
        /// </summary>
        /// <param name="title">The title-</param>
        /// <param name="subtitle">Optional subtitle.</param>
        /// <param name="order">The group order.</param>
        public TitleGroupExAttribute(string title, string subtitle = "", float order = 0f)
            : base(title, order)
        {
            Subtitle = subtitle;
            TitleAlignment = TitleAlignments.Left;
            HorizontalLine = true;
            BoldTitle = true;
            Indent = false;
        }

        /// <summary>
        /// Combines TitleGroup attributes.
        /// </summary>
        /// <param name="other">The other group attribute to combine with.</param>
        protected override void CombineValuesWith(PropertyGroupAttribute other)
        {
            var t = other as TitleGroupExAttribute;
            if (Subtitle.IsNotNullOrEmpty())
            {
                t.Subtitle = Subtitle;
            }
            else
            {
                Subtitle = t.Subtitle;
            }

            if (TitleAlignment != 0)
            {
                t.TitleAlignment = TitleAlignment;
            }
            else
            {
                TitleAlignment = t.TitleAlignment;
            }

            if (!HorizontalLine)
            {
                t.HorizontalLine = HorizontalLine;
            }
            else
            {
                HorizontalLine = t.HorizontalLine;
            }

            if (!BoldTitle)
            {
                t.BoldTitle = BoldTitle;
            }
            else
            {
                BoldTitle = t.BoldTitle;
            }

            if (Indent)
            {
                t.Indent = Indent;
            }
            else
            {
                Indent = t.Indent;
            }
        }
    }
}
