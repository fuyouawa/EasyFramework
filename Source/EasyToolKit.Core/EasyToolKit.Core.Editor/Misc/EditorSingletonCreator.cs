using System.IO;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    internal class EditorSingletonCreator
    {
        public static T GetScriptableObject<T>(string assetDirectory, string assetName)
            where T : ScriptableObject, IUnitySingleton
        {
            if (!assetDirectory.StartsWith("Assets/"))
            {
                assetDirectory = "Assets/" + assetDirectory;
            }

            var assetFilePath = assetDirectory + assetName + ".asset";
            
            var instance = AssetDatabase.LoadAssetAtPath<T>(assetFilePath);

            if (instance != null)
                return instance;

            var relocatedScriptableObject = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            if (relocatedScriptableObject.Length != 0)
            {
                instance = AssetDatabase.LoadAssetAtPath<T>(
                    AssetDatabase.GUIDToAssetPath(relocatedScriptableObject[0]));
                if (instance != null)
                {
                    instance.OnSingletonInit(SingletonInitialModes.Load);
                    return instance;
                }
            }
                
            instance = ScriptableObject.CreateInstance<T>();

            if (!Directory.Exists(assetDirectory))
            {
                Directory.CreateDirectory(new DirectoryInfo(assetDirectory).FullName);
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(instance, assetFilePath);
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            instance.OnSingletonInit(SingletonInitialModes.Create);

            return instance;
        }

    }
}
