using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    public static class GUIContentExtensions
    {
        public static GUIContent SetText(this GUIContent content, string text)
        {
            content.text = text;
            return content;
        }

        public static GUIContent SetImage(this GUIContent content, Texture image)
        {
            content.image = image;
            return content;
        }

        public static GUIContent SetTooltip(this GUIContent content, string tooltip)
        {
            content.tooltip = tooltip;
            return content;
        }
    }
}
