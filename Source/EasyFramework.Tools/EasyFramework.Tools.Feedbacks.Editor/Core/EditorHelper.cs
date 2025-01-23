using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    internal static class EditorHelper
    {
        private static readonly GUIContent s_tempContent = new GUIContent();

        public static GUIContent TempContent(string content, string tooltip = null)
        {
            s_tempContent.text = content;
            s_tempContent.image = null;
            s_tempContent.tooltip = tooltip;
            return s_tempContent;
        }
        private static readonly GUIContent s_tempContent2 = new GUIContent();

        public static GUIContent TempContent2(string content, string tooltip = null)
        {
            s_tempContent2.text = content;
            s_tempContent2.image = null;
            s_tempContent2.tooltip = tooltip;
            return s_tempContent2;
        }
    }
}
