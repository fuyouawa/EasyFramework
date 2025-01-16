using System.IO;

namespace EasyGameFramework
{
    public static class AssetsPath
    {
        public static readonly string PluginsDir;
        public static readonly string EditorResDirectory;

        static AssetsPath()
        {
            PluginsDir = "Plugins/EasyFramework";
            EditorResDirectory = Path.Combine(PluginsDir, "Editor/Resources");
        }
    }
}
