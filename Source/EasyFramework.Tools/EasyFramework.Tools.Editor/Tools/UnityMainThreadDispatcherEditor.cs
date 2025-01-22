using UnityEditor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    public static class UnityMainThreadDispatcherEditor
    {
        [MenuItem("GameObject/EasyFramework/初始化场景", false, 0)]
        private static void InitUnityInvoker(MenuCommand menuCommand)
        {
            var go = new GameObject("UnityMainThreadDispatcher");
            go.AddComponent<UnityMainThreadDispatcher>();
            
            UnityEditor.GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            
            Undo.RegisterCreatedObjectUndo(go, "Create UnityMainThreadDispatcher");

            Selection.activeObject = go;
        }

        [MenuItem("GameObject/EasyFramework/初始化场景", true)]
        private static bool InitUnityInvokerValidation()
        {
            return Object.FindAnyObjectByType<UnityMainThreadDispatcher>() == null;
        }
    }
}
