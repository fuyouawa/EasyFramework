using System;
using System.IO;
using EasyFramework;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EasyGameFramework
{
    internal partial class SingletonCreator
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
#if UNITY_EDITOR

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

#else
            if (instance == null)
            {
                throw new Exception($"加载ScriptableObject：{typeof(T).Name}失败！");
            }
#endif

            return instance;
        }

        internal static void LoadInstanceIfAssetExists(string assetPath, string defaultFileNameWithoutExtension = null)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ScriptableObjectSingletonAssetPathAttribute : Attribute
    {
        public string AssetDirectory;

        public string AssetName;

        public ScriptableObjectSingletonAssetPathAttribute(string assetDirectory, string assetName)
        {
            AssetDirectory = assetDirectory.Trim().TrimEnd('/', '\\').TrimStart('/', '\\')
                .Replace('\\', '/') + "/";

            AssetName = assetName;
        }

        public ScriptableObjectSingletonAssetPathAttribute(string assetPath)
            : this(assetPath, string.Empty)
        {
        }
    }

    public class ScriptableObjectSingleton<T> : ScriptableObject
        where T : ScriptableObjectSingleton<T>, new()
    {
        private static ScriptableObjectSingletonAssetPathAttribute _assetPathAttribute;

        private static T _instance;

        public static ScriptableObjectSingletonAssetPathAttribute AssetPathAttribute
        {
            get
            {
                if (_assetPathAttribute == null)
                {
                    _assetPathAttribute = typeof(T).GetCustomAttribute<ScriptableObjectSingletonAssetPathAttribute>();
                    if (_assetPathAttribute == null)
                    {
                        throw new Exception($"{typeof(T).Name}必须定义一个[ScriptableObjectSingletonAssetPath]的Attribute!");
                    }
                }

                return _assetPathAttribute;
            }
        }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = SingletonCreator.GetScriptableObjectSingleton<T>(AssetPathAttribute.AssetDirectory,
                        AssetPathAttribute.AssetName.IsNotNullOrEmpty()
                            ? AssetPathAttribute.AssetName
                            : typeof(T).Name);
                }

                return _instance;
            }
        }
    }
}
