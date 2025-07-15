using EasyToolKit.Core.Internal;

namespace EasyToolKit.Core.Editor.Internal
{
    public static class EditorAssetPaths
    {
        public static string GetAssetDirectory(string folderName, string assetType)
        {
            return $"{AssetPaths.PluginsDirectory}/{folderName}/{assetType}/Editor";
        }


        public static string GetModuleAssetDirectory(string folderName, string assetType)
        {
            return $"{AssetPaths.PluginsDirectory}/Modules/{folderName}/{assetType}/Editor";
        }
    }


    public class EditorConfigsPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public EditorConfigsPathAttribute(string folderName) : base(
            EditorAssetPaths.GetAssetDirectory(folderName, "Configs"))
        {
        }
    }

    public class ModuleEditorConfigsPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public ModuleEditorConfigsPathAttribute(string folderName) : base(
            EditorAssetPaths.GetModuleAssetDirectory(folderName, "Configs"))
        {
        }
    }
}
