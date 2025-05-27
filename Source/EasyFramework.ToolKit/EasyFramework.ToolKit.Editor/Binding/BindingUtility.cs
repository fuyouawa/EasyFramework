using EasyFramework.Core;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace EasyFramework.ToolKit.Editor
{
    public static class BindingUtility
    {
        public static bool IsValidIdentifier(string identifier)
        {
            identifier = identifier.Trim();

            if (identifier.IsNullOrEmpty())
                return false;

            if (!char.IsLetter(identifier[0]) && identifier[0] != '_')
            {
                return false;
            }

            if (identifier.Length == 1)
                return true;

            var other = identifier[1..];
            foreach (var ch in other)
            {
                if (!char.IsLetterOrDigit(ch) && ch != '_')
                {
                    return false;
                }
            }

            return true;
        }

        [MenuItem("GameObject/EasyFramework/Add Binder", false, -100)]
        private static void AddBinder()
        {
            foreach (var o in Selection.gameObjects)
            {
                if (o.GetComponent<Binder>() != null)
                    continue;

                o.AddComponent<Binder>();
                EditorUtility.SetDirty(o);
                EditorSceneManager.MarkSceneDirty(o.scene);
            }
        }

        [MenuItem("GameObject/EasyFramework/Add Builder", false, -100)]
        private static void AddBuilder()
        {
            foreach (var o in Selection.gameObjects)
            {
                if (o.GetComponent<Builder>() != null)
                    continue;

                o.AddComponent<Builder>();
                EditorUtility.SetDirty(o);
                EditorSceneManager.MarkSceneDirty(o.scene);
            }
        }
    }
}
