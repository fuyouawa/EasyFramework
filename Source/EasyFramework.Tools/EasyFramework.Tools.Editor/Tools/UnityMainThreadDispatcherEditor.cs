using UnityEditor;
using UnityEngine;

namespace EasyGameFramework.Editor
{
    public static class UnityMainThreadDispatcherEditor
    {
        [MenuItem("GameObject/EasyGameFramework/Initialize UnityInvoker", false, 0)]
        private static void InitUnityInvoker(MenuCommand menuCommand)
        {
            var go = new GameObject("UnityMainThreadDispatcher");
            go.AddComponent<UnityMainThreadDispatcher>();
            
            UnityEditor.GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            
            Undo.RegisterCreatedObjectUndo(go, "Create UnityMainThreadDispatcher");

            Selection.activeObject = go;
        }

        [MenuItem("GameObject/EasyGameFramework/Initialize UnityInvoker", true)]
        private static bool InitUnityInvokerValidation()
        {
            return Object.FindAnyObjectByType<UnityMainThreadDispatcher>() == null;
        }
    }
}
