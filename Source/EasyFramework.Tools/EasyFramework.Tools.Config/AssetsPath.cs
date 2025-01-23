using EasyFramework.Utilities;
using System.IO;

namespace EasyFramework.Tools
{
    internal static class AssetsPath
    {
        public static readonly string PluginsDir;
        public static readonly string ConfigsDirectory;

        static AssetsPath()
        {
            PluginsDir = "Plugins/EasyFramework";
            ConfigsDirectory = PluginsDir + "/Configs/Resources";
        }
    }

    public class ConfigAssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public ConfigAssetPathAttribute() : base(AssetsPath.ConfigsDirectory)
        {
        }
    }
}
