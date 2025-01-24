using System;
using System.Reflection;
using UnityEngine;

namespace EasyFramework
{
    internal partial class SingletonCreator
    {
        public static T GetScriptableObject<T>(string assetDirectory, string assetName)
            where T : ScriptableObject, IUnitySingleton
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
            where T : ScriptableObject, IUnitySingleton
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
            instance.OnSingletonInit(SingletonInitialModes.Load);

            return instance;
        }

        private static MethodInfo _getScriptableObjectSingletonInEditorMethod;

        private static T GetScriptableObjectInEditor<T>(string assetDirectory, string assetName)
            where T : ScriptableObject, IUnitySingleton
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
                        throw new Exception($"{typeof(T).Name}必须定义一个[ScriptableObjectSingletonAssetPath]的Attribute!");
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
