using System.IO;
using EasyFramework.Utilities;
using UnityEditor;

namespace EasyFramework.Tools.Editor
{
    public static class EditorAssetsPath
    {
        public static readonly string TempDirectory;
        public static readonly string ConfigsDirectory;
        public static readonly string UiTextManagerWindowTempPath;


        static EditorAssetsPath()
        {
            TempDirectory = Path.Combine(Path.GetTempPath(), PlayerSettings.productName);
            ConfigsDirectory = AssetsPath.EditorConfigsDirectory;
            UiTextManagerWindowTempPath = Path.Combine(TempDirectory, "UiTextManager.WindowTemp.json");

            System.IO.Directory.CreateDirectory(TempDirectory);
        }
    }
    public class EditorConfigAssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public EditorConfigAssetPathAttribute() : base(EditorAssetsPath.ConfigsDirectory)
        {
        }
    }
}
