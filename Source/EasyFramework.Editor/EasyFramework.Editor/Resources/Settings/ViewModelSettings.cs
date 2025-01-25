using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.Editor
{
    [EditorSettingsAssetPath]
    public class ViewModelSettings : ScriptableObjectSingleton<ViewModelSettings>
    {
        [TitleCN("默认值设置")]
        [LabelText("生成路径")]
        [FolderPath(ParentFolder = "Assets")]
        public string DefaultGenerateDirectory = "Scripts";

        [LabelText("命名空间")]
        public string DefaultNamespace;

        [TitleCN("类内容生成模板")]
        [LabelText("自动缩进")]
        public bool AutoIndent = true;
        [HideLabel]
        [TextArea(5, 10)]
        public string ClassTemplate;
    }
}
