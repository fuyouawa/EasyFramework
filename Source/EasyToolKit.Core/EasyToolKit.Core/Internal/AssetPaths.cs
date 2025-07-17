
namespace EasyToolKit.Core.Internal
{
    public static class AssetPaths
    {
        public static readonly string PluginsDirectory = "Plugins/EasyToolKit";

        public static string GetAssetDirectory(string assetType)
        {
            return $"{PluginsDirectory}/{assetType}/Resources";
        }

        public static string GetModuleAssetDirectory(string folderName, string assetType)
        {
            return $"{PluginsDirectory}/Modules/{folderName}/{assetType}/Resources";
        }
    }

    public class ConfigsPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public ConfigsPathAttribute() : base(AssetPaths.GetAssetDirectory("Configs"))
        {
        }
    }

    public class ModuleConfigsPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public ModuleConfigsPathAttribute(string folderName) : base(AssetPaths.GetModuleAssetDirectory(folderName, "Configs"))
        {
        }
    }
}
