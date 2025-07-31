
namespace EasyToolKit.Core.Internal
{
    public static class AssetPaths
    {
        public static string GetPluginsDirectory()
        {
            return "Plugins/EasyToolKit";
        }

        public static string GetModuleAssetDirectory(string moduleName)
        {
            return $"{GetPluginsDirectory()}/{moduleName}/Assets";
        }

        public static string GetModuleResourcesDirectory(string moduleName)
        {
            return $"{GetModuleAssetDirectory(moduleName)}/Resources";
        }

        public static string GetModuleConfigsDirectory(string moduleName)
        {
            return $"{GetModuleResourcesDirectory(moduleName)}/Configs";
        }
    }

    public class ModuleConfigsPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public ModuleConfigsPathAttribute(string moduleName) : base(AssetPaths.GetModuleConfigsDirectory(moduleName))
        {
        }
    }
}
