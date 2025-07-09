using System;
using System.Reflection;
using UnityEngine;

namespace EasyToolKit.Core
{
    internal partial class SingletonCreator
    {
        public static T GetScriptableObject<T>(string assetDirectory, string assetName)
            where T : ScriptableObject, IUnitySingleton
        {
            if (!assetDirectory.Equals("resources/", StringComparison.OrdinalIgnoreCase) &&
                !assetDirectory.Contains("/resources/", StringComparison.OrdinalIgnoreCase))
            {
                if (!assetDirectory.Contains("/editor/", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException(
                        $"The path '{assetDirectory}/{assetName}' must be inside a 'Resources' folder.");
                }
                else
                {
                    if (!Application.isEditor)
                    {
                        throw new ArgumentException(
                            $"The editor asserts '{assetDirectory}/{assetName}' can only be loaded in edit mode!");
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
            where T : ScriptableObject, IUnitySingleton
        {
            string resourcesPath = assetDirectory;
            int i = resourcesPath.LastIndexOf("/resources/", StringComparison.OrdinalIgnoreCase);
            if (i >= 0)
            {
                resourcesPath = resourcesPath[(i + "/resources/".Length)..];
            }
            else
            {
                resourcesPath = resourcesPath["resources/".Length..];
            }

            var instance = Resources.Load<T>(resourcesPath + assetName);

            if (instance == null)
            {
                throw new Exception($"Load ScriptableObject '{typeof(T).Name}' failed！");
            }

            instance.OnSingletonInit(SingletonInitialModes.Load);

            return instance;
        }

        private static MethodInfo _getScriptableObjectSingletonInEditorMethod;

        private static T GetScriptableObjectInEditor<T>(string assetDirectory, string assetName)
            where T : ScriptableObject, IUnitySingleton
        {
            if (_getScriptableObjectSingletonInEditorMethod == null)
            {
                var type = Type.GetType("EasyFramework.Editor.EditorSingletonCreator, EasyFramework.Editor")!;
                _getScriptableObjectSingletonInEditorMethod =
                    type.GetMethod("GetScriptableObject", BindingFlags.Static | BindingFlags.Public)!;
            }

            var m = _getScriptableObjectSingletonInEditorMethod.MakeGenericMethod(typeof(T));
            return (T)m.Invoke(null, new object[] { assetDirectory, assetName });
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

    public class ScriptableObjectSingleton<T> : ScriptableObject, IUnitySingleton
        where T : ScriptableObjectSingleton<T>, new()
    {
        private static ScriptableObjectSingletonAssetPathAttribute s_assetPathAttribute;

        private static T s_instance;

        public static ScriptableObjectSingletonAssetPathAttribute AssetPathAttribute
        {
            get
            {
                if (s_assetPathAttribute == null)
                {
                    s_assetPathAttribute = typeof(T).GetCustomAttribute<ScriptableObjectSingletonAssetPathAttribute>();
                    if (s_assetPathAttribute == null)
                    {
                        throw new Exception(
                            $"Type {typeof(T).Name} must define a 'ScriptableObjectSingletonAssetPath' Attribute!");
                    }
                }

                return s_assetPathAttribute;
            }
        }

        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = SingletonCreator.GetScriptableObject<T>(AssetPathAttribute.AssetDirectory,
                        AssetPathAttribute.AssetName.IsNotNullOrEmpty()
                            ? AssetPathAttribute.AssetName
                            : typeof(T).Name);
                }

                return s_instance;
            }
        }

        public virtual void OnSingletonInit(SingletonInitialModes mode)
        {
        }
    }
}
