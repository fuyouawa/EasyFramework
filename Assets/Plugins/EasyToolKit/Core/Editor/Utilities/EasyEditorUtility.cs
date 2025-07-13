using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    public static class EasyEditorUtility
    {
        public static void SetUnityObjectDirty(Object unityObject)
        {
            if (AssetDatabase.Contains(unityObject))
            {
                EditorUtility.SetDirty(unityObject);
                return;
            }

            if (Application.isPlaying)
                return;

            switch (unityObject)
            {
                case Component target:
                    EditorUtility.SetDirty(target);
                    EditorUtility.SetDirty(target.gameObject);
                    EditorSceneManager.MarkSceneDirty(target.gameObject.scene);
                    break;
                case EditorWindow _:
                case ScriptableObject _:
                    EditorUtility.SetDirty(unityObject);
                    break;
                default:
                    EditorUtility.SetDirty(unityObject);
                    EditorSceneManager.MarkAllScenesDirty();
                    break;
            }
        }
    }
}
