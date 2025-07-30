using UnityEngine;

namespace EasyToolKit.Inspector
{
    public class TitleAttribute : PropertyAttribute
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