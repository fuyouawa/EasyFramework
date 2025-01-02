using System.IO;
using UnityEditor;
using UnityEngine;

namespace EasyGameFramework.Editor
{
    public static class EditorAssetsPath
    {
        public static readonly string TempDirectory;
        public static readonly string ResDirectory;

        public static readonly string UiTextAssetsEditorSettingsPath;

        public static readonly string UiTextAssetsManagerWindowTempPath;


        static EditorAssetsPath()
        {
            TempDirectory = Path.Combine(Path.GetTempPath(), PlayerSettings.productName);
            ResDirectory = Path.Combine(Application.dataPath, "Plugins/EasyGameFramework/Editor/Resources");
            UiTextAssetsEditorSettingsPath = Path.Combine(ResDirectory, "UiTextAssetsEditorSettings.json");
            UiTextAssetsManagerWindowTempPath = Path.Combine(TempDirectory, "UiTextAssetsManagerWindowTemp.json");

            Directory.CreateDirectory(TempDirectory);
        }
    }
}
