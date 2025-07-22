using EasyToolKit.Core.Internal;

namespace EasyToolKit.Core.Editor.Internal
{
    public static class EditorAssetPaths
    {
        public static string GetAssetDirectory()
        {
            return GetDirectory("Assets");
        }
        public static string GetModuleAssetDirectory(string folderName)
        {
            return GetModuleDirectory(folderName, "Assets");
        }

        public static string GetConfigsDirectory()
        {
            return GetDirectory("Configs");
        }

        public static string GetModuleConfigsDirectory(string folderName)
        {
            return GetModuleDirectory(folderName, "Configs");
        }

        private static string GetDirectory(string assetType)
        {
            return $"Assets/{AssetPaths.GetPluginsDirectory()}/{assetType}/Editor";
        }


        private static string GetModuleDirectory(string folderName, string assetType)
        {
            return $"Assets/{AssetPaths.GetPluginsDirectory()}/Modules/{folderName}/{assetType}/Editor";
        }
    }

    public class EditorAssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public EditorAssetPathAttribute() : base(EditorAssetPaths.GetAssetDirectory())
        {
        }
    }

    public class EditorModuleAssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public EditorModuleAssetPathAttribute(string folderName) : base(EditorAssetPaths.GetModuleAssetDirectory(folderName))
        {
        }
    }


    public class EditorConfigsPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public EditorConfigsPathAttribute() : base(
            EditorAssetPaths.GetConfigsDirectory())
        {
        }
    }

    public class ModuleEditorConfigsPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public ModuleEditorConfigsPathAttribute(string folderName) : base(
            EditorAssetPaths.GetModuleAssetDirectory(folderName))
        {
        }
    }
}
