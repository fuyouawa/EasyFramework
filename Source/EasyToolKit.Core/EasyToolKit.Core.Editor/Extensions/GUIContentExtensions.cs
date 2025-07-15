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
    }
}
