using EasyFramework.Utilities;
using System.IO;

namespace EasyFramework.Tools
{
    public static class AssetsPath
    {
        public static readonly string PluginsDir;
        public static readonly string ConfigsDirectory;
        public static readonly string EditorConfigsDirectory;

        static AssetsPath()
        {
            PluginsDir = "Plugins/EasyFramework";
            ConfigsDirectory = PluginsDir + "/Configs/Resources";
            EditorConfigsDirectory = PluginsDir + "/Configs/Editor";
        }
    }

    public class ConfigAssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public ConfigAssetPathAttribute() : base(AssetsPath.ConfigsDirectory)
        {
        }
    }
}
