using EasyFramework.Core;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor
{
    public static class EditorWindowExtension
    {
        public static void CenterWindowWithRadio(this EditorWindow window, float widthRadio, float heightRadio)
        {
            var rect = GUIHelper.GetEditorWindowRect();
            window.CenterWindow(rect.width * widthRadio, rect.height * heightRadio);
        }

        public static void CenterWindow(this EditorWindow window, float width, float height)
        {
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(width, height);
        }
    }
}
