using System.IO;

namespace EasyFramework.Tools
{
    public static class AssetsPath
    {
        public static readonly string PluginsDir;
        public static readonly string EditorConfigsDirectory;

        static AssetsPath()
        {
            PluginsDir = "Plugins/EasyFramework";
            EditorConfigsDirectory = Path.Combine(PluginsDir, "Configs/Editor");
        }
    }
}
