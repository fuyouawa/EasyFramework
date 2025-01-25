using Sirenix.OdinInspector;

namespace EasyFramework.Editor
{
    [EditorSettingsAssetPath]
    public class ViewModelSettings : ScriptableObjectSingleton<ViewModelSettings>
    {
        [LabelText("生成路径")]
        [FolderPath(ParentFolder = "Assets")]
        public string GenerateDir = "Scripts/Ui";

        [LabelText("命名空间")]
        public string Namespace;
    }
}
