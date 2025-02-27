using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [EditorSettingsAssetPath]
    public class ViewControllerSettings : ScriptableObjectSingleton<ViewControllerSettings>
    {
        [Serializable, InlineProperty, HideReferenceObjectPicker, HideLabel]
        public class DefaultSettings
        {
            [LabelText("生成路径")]
            [FolderPath(ParentFolder = "Assets")]
            public string GenerateDir = "Scripts";

            [LabelText("命名空间")]
            public string Namespace;

            [LabelText("基类")]
            [ShowInInspector]
            [TypeDrawerSettings(BaseType = typeof(Component), Filter = TYPE_FILTER)]
            public Type BaseType;
        }

        [TitleEx("默认值设置")]
        public DefaultSettings Default;

        [TitleEx("类内容生成模板")]
        [LabelText("自动缩进")]
        public bool AutoIndent = true;
        [HideLabel]
        [TextArea(5, 10)]
        public string ClassTemplate;


        private const TypeInclusionFilter TYPE_FILTER =
            TypeInclusionFilter.IncludeConcreteTypes | TypeInclusionFilter.IncludeAbstracts;
    }
}
