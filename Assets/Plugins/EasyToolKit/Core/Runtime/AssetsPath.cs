
namespace EasyToolKit.Core.Internal
{
    public static class AssetsPath
    {
        public static readonly string PluginsDirectory = "Plugins/EasyToolKit";

        public static string GetResourcesDirectory(string folderName)
        {
            return $"{PluginsDirectory}/{folderName}/Resources";
        }

        public static string GetModuleResourcesDirectory(string folderName)
        {
            return $"{PluginsDirectory}/Modules/{folderName}/Resources";
        }

        public static string GetAssetDirectory(string folderName, string assetType)
        {
            return $"{GetResourcesDirectory(folderName)}/{assetType}";
        }

        public static string GetModuleAssetDirectory(string folderName, string assetType)
        {
            return $"{GetModuleResourcesDirectory(folderName)}/{assetType}";
        }
    }
}
