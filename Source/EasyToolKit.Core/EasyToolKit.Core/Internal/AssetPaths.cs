
namespace EasyToolKit.Core.Internal
{
    public static class AssetPaths
    {
        public static string GetPluginsDirectory()
        {
            return "Plugins/EasyToolKit";
        }

        public static string GetAssetDirectory()
        {
            return GetDirectory("Assets");
        }

        public static string GetConfigsDirectory()
        {
            return GetDirectory("Configs");
        }

        public static string GetModuleAssetDirectory(string folderName)
        {
            return GetModuleDirectory(folderName, "Assets");
        }

        public static string GetModuleConfigsDirectory(string folderName)
        {
            return GetModuleDirectory(folderName, "Configs");
        }

        private static string GetDirectory(string assetType)
        {
            return $"{GetPluginsDirectory()}/{assetType}/Resources";
        }

        private static string GetModuleDirectory(string folderName, string assetType)
        {
            return $"{GetPluginsDirectory()}/Modules/{folderName}/{assetType}/Resources";
        }
    }

    public class AssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public AssetPathAttribute() : base(AssetPaths.GetAssetDirectory())
        {
        }
    }

    public class ModuleAssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public ModuleAssetPathAttribute(string folderName) : base(AssetPaths.GetModuleAssetDirectory(folderName))
        {
        }
    }

    public class ConfigsPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public ConfigsPathAttribute() : base(AssetPaths.GetConfigsDirectory())
        {
        }
    }

    public class ModuleConfigsPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public ModuleConfigsPathAttribute(string folderName) : base(AssetPaths.GetModuleConfigsDirectory(folderName))
        {
        }
    }
}
