using System.IO;
using UnityEditor;

namespace EasyFramework.ToolKit.Editor
{
    internal static class EditorAssetsPath
    {
        public static readonly string PluginsDirectory;
        public static readonly string ConfigsDirectory;
        public static readonly string SettingsDirectory;
        public static readonly string DataDirectory;

        public static readonly string TempDirectory;
        public static readonly string UiTextManagerWindowTempPath;


        static EditorAssetsPath()
        {
            PluginsDirectory = "Plugins/EasyFramework";
            ConfigsDirectory = PluginsDirectory + "/Editor/Configs";
            SettingsDirectory = PluginsDirectory + "/Editor/Settings";
            DataDirectory = PluginsDirectory + "/Editor/Data";

            TempDirectory = Path.Combine(Path.GetTempPath(), PlayerSettings.productName);
            UiTextManagerWindowTempPath = Path.Combine(TempDirectory, "UiTextManager.WindowTemp.json");

            Directory.CreateDirectory(TempDirectory);
        }
    }

    internal class EditorConfigsAssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public EditorConfigsAssetPathAttribute() : base(EditorAssetsPath.ConfigsDirectory)
        {
        }
    }

    internal class EditorSettingsAssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public EditorSettingsAssetPathAttribute() : base(EditorAssetsPath.SettingsDirectory)
        {
        }
    }

    internal class EditorDataAssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public EditorDataAssetPathAttribute() : base(EditorAssetsPath.DataDirectory)
        {
        }
    }
}
