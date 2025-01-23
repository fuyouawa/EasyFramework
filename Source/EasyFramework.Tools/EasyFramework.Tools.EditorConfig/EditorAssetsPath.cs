using System.IO;
using EasyFramework.Utilities;
using UnityEditor;

namespace EasyFramework.Tools.Editor
{
    public static class EditorAssetsPath
    {
        public static readonly string PluginsDir;
        public static readonly string ConfigsDirectory;

        public static readonly string TempDirectory;
        public static readonly string UiTextManagerWindowTempPath;


        static EditorAssetsPath()
        {
            PluginsDir = "Plugins/EasyFramework";
            ConfigsDirectory = PluginsDir + "/Configs/Editor";

            TempDirectory = Path.Combine(Path.GetTempPath(), PlayerSettings.productName);
            UiTextManagerWindowTempPath = Path.Combine(TempDirectory, "UiTextManager.WindowTemp.json");

            Directory.CreateDirectory(TempDirectory);
        }
    }
    public class EditorConfigAssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public EditorConfigAssetPathAttribute() : base(EditorAssetsPath.ConfigsDirectory)
        {
        }
    }
}
