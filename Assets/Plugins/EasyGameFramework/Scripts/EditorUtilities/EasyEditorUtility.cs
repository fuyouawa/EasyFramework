#if UNITY_EDITOR
using EasyFramework;
using UnityEditor;

namespace EasyGameFramework
{
    public static class EasyEditorUtility
    {
        public static void ForceRebuildInspectors()
        {
            typeof(EditorUtility).InvokeMethod("ForceRebuildInspectors", null);
        }
    }
}

#endif