namespace EasyFramework
{
    public static class AssetsPath
    {
        public static readonly string PluginsDirectory;
        public static readonly string ResourcesDirectory;

        public static readonly string ConfigsDirectory;
        public static readonly string DataDirectory;

        static AssetsPath()
        {
            PluginsDirectory = "Plugins/EasyFramework";
            ResourcesDirectory = PluginsDirectory + "/Resources";
            ConfigsDirectory = ResourcesDirectory + "/Configs";
            DataDirectory = ResourcesDirectory + "/Data";
        }
    }

    internal class ConfigsAssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public ConfigsAssetPathAttribute() : base(AssetsPath.ConfigsDirectory)
        {
        }
    }
}
