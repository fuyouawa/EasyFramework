using EasyToolKit.Core.Internal;

namespace EasyToolKit.Core.Editor.Internal
{
    public static class EditorAssetPaths
    {
        public static string GetModuleEditorDirectory(string moduleName)
        {
            return $"Assets/{AssetPaths.GetModuleAssetDirectory(moduleName)}/Editor";
        }

        public static string GetModuleConfigsDirectory(string moduleName)
        {
            return $"{GetModuleEditorDirectory(moduleName)}/Configs";
        }
    }

    public class ModuleEditorConfigsPathAttribute : ScriptableObjectSingletonAssetPathAttribute
    {
        public ModuleEditorConfigsPathAttribute(string moduleName) : base(
            EditorAssetPaths.GetModuleConfigsDirectory(moduleName))
        {
        }
    }
}
