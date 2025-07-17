using EasyToolKit.Core.Internal;

namespace EasyToolKit.Core.Editor.Internal
{
    public static class EditorAssetPaths
    {
        public static string GetAssetDirectory(string assetType)
        {
            return $"{AssetPaths.PluginsDirectory}/{assetType}/Editor";
        }


        public static string GetModuleAssetDirectory(string folderName, string assetType)
        {
            return $"{AssetPaths.PluginsDirectory}/Modules/{folderName}/{assetType}/Editor";
        }
    }


    public class EditorConfigsPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public EditorConfigsPathAttribute() : base(
            EditorAssetPaths.GetAssetDirectory("Configs"))
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
