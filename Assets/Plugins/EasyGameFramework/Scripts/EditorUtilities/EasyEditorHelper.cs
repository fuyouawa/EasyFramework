#if UNITY_EDITOR
using EasyFramework;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace EasyGameFramework
{
    public static class EasyEditorHelper
    {
        public static void ForceRebuildInspectors()
        {
            typeof(EditorUtility).InvokeMethod("ForceRebuildInspectors", null);
        }

        public static float Indent => GUIHelper.CurrentIndentAmount;
    }
}

#endif
