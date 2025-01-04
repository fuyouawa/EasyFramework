using System.IO;
using UnityEditor;
using UnityEngine;

namespace EasyGameFramework.Editor
{
    public static class EditorAssetsPath
    {
        public static readonly string TempDirectory;
        public static readonly string ResDirectory;

        public static readonly string UiTextManagerSettingsPath;

        public static readonly string UiTextManagerWindowTempPath;


        static EditorAssetsPath()
        {
            TempDirectory = Path.Combine(Path.GetTempPath(), PlayerSettings.productName);
            ResDirectory = Path.Combine(Application.dataPath, "Plugins/EasyGameFramework/Editor/Resources");
            UiTextManagerSettingsPath = Path.Combine(ResDirectory, "UiTextManager.Settings.json");
            UiTextManagerWindowTempPath = Path.Combine(TempDirectory, "UiTextManager.WindowTemp.json");

            Directory.CreateDirectory(TempDirectory);
        }
    }
}
