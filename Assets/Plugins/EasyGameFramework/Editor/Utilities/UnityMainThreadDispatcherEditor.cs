using UnityEditor;
using UnityEngine;

namespace EasyGameFramework.Editor
{
    public static class UnityMainThreadDispatcherEditor
    {
        [MenuItem("GameObject/UniTool/Manual initialize UnityMainThreadDispatcher", false, 0)]
        private static void InitUnityInvoker(MenuCommand menuCommand)
        {
            if (Object.FindAnyObjectByType<UnityMainThreadDispatcher>() == null)
            {
                var go = new GameObject("UnityMainThreadDispatcher");
                go.AddComponent<UnityMainThreadDispatcher>();
            
                UnityEditor.GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            
                Undo.RegisterCreatedObjectUndo(go, "Create UnityMainThreadDispatcher");

                Selection.activeObject = go;
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "场景中已经存在一个UnityMainThreadDispatcher！", "确认");
            }
        }
    }
}
