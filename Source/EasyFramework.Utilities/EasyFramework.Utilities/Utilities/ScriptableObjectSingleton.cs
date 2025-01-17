using System;
using EasyFramework.Generic;
using System.Reflection;
using UnityEngine;

namespace EasyFramework.Utilities
{
    internal partial class SingletonCreator
    {
        public static T GetScriptableObject<T>(string assetDirectory, string assetName)
            where T : ScriptableObject
        {
            if (!assetDirectory.Contains("/resources/", StringComparison.OrdinalIgnoreCase))
            {
                if (!assetDirectory.Contains("/editor/", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException(
                        $"\"{assetDirectory + '/' + assetName}\"'s resource path must be under the Resources directory！");
                }
                else
                {
                    if (!Application.isEditor)
                    {
                        throw new ArgumentException(
                            $"editor asserts({assetDirectory + '/' + assetName}) can only be loaded in edit mode!");
                    }
                }
            }

            if (Application.isEditor)
            {
                return GetScriptableObjectInEditor<T>(assetDirectory, assetName);
            }
            else
            {
                return InternalGetScriptableObject<T>(assetDirectory, assetName);
            }
        }

        private static T InternalGetScriptableObject<T>(string assetDirectory, string assetName)
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
                resourcesPath = resourcesPath[(i + "/resources/".Length)..];
            }

            var instance = Resources.Load<T>(resourcesPath + assetName);

            if (instance == null)
            {
                throw new Exception($"加载ScriptableObject：{typeof(T).Name}失败！");
            }

            return instance;
        }

        private static MethodInfo _getScriptableObjectSingletonInEditorMethod;

        private static T GetScriptableObjectInEditor<T>(string assetDirectory, string assetName)
            where T : ScriptableObject
        {
            if (_getScriptableObjectSingletonInEditorMethod == null)
            {
                var type = Type.GetType("EasyFramework.Inspector.EditorSingletonCreator, EasyFramework.Inspector")!;
                _getScriptableObjectSingletonInEditorMethod =
                    type.GetMethod("GetScriptableObject", BindingFlags.Static | BindingFlags.Public)!;
            }

            var m = _getScriptableObjectSingletonInEditorMethod.MakeGenericMethod(typeof(T));
            return (T)m.Invoke(null, new object[] { assetDirectory, assetName });
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
                    _instance = SingletonCreator.GetScriptableObject<T>(AssetPathAttribute.AssetDirectory,
                        AssetPathAttribute.AssetName.IsNotNullOrEmpty()
                            ? AssetPathAttribute.AssetName
                            : typeof(T).Name);
                }

                return _instance;
            }
        }
    }
}
