using System.IO;
using UnityEditor;
using UnityEngine;

namespace EasyGameFramework.Editor
{
    public static class EditorAssetsPath
    {
        public static readonly string TempDirectory;
        public static readonly string ResDirectory;
        public static readonly string UiTextManagerWindowTempPath;


        static EditorAssetsPath()
        {
            TempDirectory = Path.Combine(Path.GetTempPath(), PlayerSettings.productName);
            ResDirectory = AssetsPath.EditorResDirectory;
            UiTextManagerWindowTempPath = Path.Combine(TempDirectory, "UiTextManager.WindowTemp.json");

            Directory.CreateDirectory(TempDirectory);
        }
    }
    public class EditorResourcesAssetPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public EditorResourcesAssetPathAttribute() : base(EditorAssetsPath.ResDirectory)
        {
        }
    }
}
