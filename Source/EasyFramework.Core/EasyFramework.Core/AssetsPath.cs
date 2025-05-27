
namespace EasyFramework.Core.Internal
{
    public static class AssetsPath
    {
        public static readonly string PluginsDirectory;
        public static readonly string ResourcesDirectory;

        public static readonly string ConfigsDirectory;
        public static readonly string SettingsDirectory;
        public static readonly string DataDirectory;
        
        public static readonly string EditorConfigsDirectory;
        public static readonly string EditorSettingsDirectory;
        public static readonly string EditorDataDirectory;

        static AssetsPath()
        {
            PluginsDirectory = "Plugins/EasyFramework";
            ResourcesDirectory = PluginsDirectory + "/Resources";
            ConfigsDirectory = ResourcesDirectory + "/Configs";
            SettingsDirectory = ResourcesDirectory + "/Settings";
            DataDirectory = ResourcesDirectory + "/Data";
            
            EditorConfigsDirectory = PluginsDirectory + "/Editor/Configs";
            EditorSettingsDirectory = PluginsDirectory + "/Editor/Settings";
            EditorDataDirectory = PluginsDirectory + "/Editor/Data";
        }
    }

    public class ConfigsAssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public ConfigsAssetPathAttribute() : base(AssetsPath.ConfigsDirectory)
        {
        }
    }

    public class SettingsAssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public SettingsAssetPathAttribute() : base(AssetsPath.SettingsDirectory)
        {
        }
    }

    public class EditorConfigsAssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public EditorConfigsAssetPathAttribute() : base(AssetsPath.EditorConfigsDirectory)
        {
        }
    }

    public class EditorSettingsAssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public EditorSettingsAssetPathAttribute() : base(AssetsPath.EditorSettingsDirectory)
        {
        }
    }
}
