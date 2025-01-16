using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EasyFramework
{
    internal class EditorSingletonCreator
    {
        public static T GetScriptableObjectSingleton<T>(string assetDirectory, string assetName)
            where T : ScriptableObject
        {
            if (!assetDirectory.Contains("/resources/", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"{assetName}的资源路径必须在Resources目录下！");
            }

            string resourcesPath = assetDirectory;
            int i = resourcesPath.LastIndexOf("/resources/", StringComparison.OrdinalIgnoreCase);
            if (i >= 0)
            {
                resourcesPath = resourcesPath.Substring(i + "/resources/".Length);
            }

            var instance = Resources.Load<T>(resourcesPath + assetName);

            if (!assetDirectory.StartsWith("Assets/"))
            {
                assetDirectory = "Assets/" + assetDirectory;
            }

            var assetFilePath = assetDirectory + assetName + ".asset";

            if (instance == null)
            {
                instance = AssetDatabase.LoadAssetAtPath<T>(assetFilePath);
            }

            if (instance == null)
            {
                string[] relocatedScriptableObject = AssetDatabase.FindAssets("t:" + typeof(T).Name);
                if (relocatedScriptableObject.Length != 0)
                {
                    instance = AssetDatabase.LoadAssetAtPath<T>(
                        AssetDatabase.GUIDToAssetPath(relocatedScriptableObject[0]));
                }
            }

            if (instance == null && EditorPrefs.HasKey("PREVENT_SIRENIX_FILE_GENERATION"))
            {
                Debug.LogWarning($"{assetFilePath}生成失败，由于PREVENT_SIRENIX_FILE_GENERATION被EditorPrefs所定义！");
                instance = ScriptableObject.CreateInstance<T>();
                return instance;
            }

            if (instance == null)
            {
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
            }

            return instance;
        }

    }
}
