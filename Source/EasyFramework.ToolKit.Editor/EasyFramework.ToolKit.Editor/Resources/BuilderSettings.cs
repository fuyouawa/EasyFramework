using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using EasyFramework.Core;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [EditorSettingsAssetPath]
    [ShowOdinSerializedPropertiesInInspector]
    public class BuilderSettings : ScriptableObjectSingleton<BuilderSettings>,
        ISerializationCallbackReceiver
    {
        private static Type[] s_baseTypes;

        public static Type[] BaseTypes
        {
            get
            {
                s_baseTypes ??= AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(asm => asm.GetTypes())
                    .Where(t => t.IsSubclassOf(typeof(Component)) && !t.ContainsGenericParameters && t.IsPublic)
                    .ToArray();
                return s_baseTypes;
            }
        }

        [InlineProperty, HideReferenceObjectPicker, HideLabel]
        public struct DefaultSettings
        {
            [LabelText("生成路径")]
            [FolderPath(ParentFolder = "Assets")]
            public string GenerateDir;

            [LabelText("命名空间")]
            public string Namespace;

            [LabelText("基类")]
            [ShowInInspector]
            [ValueDropdown(nameof(GetBaseTypeDropdown))]
            public Type BaseType;

            [LabelText("绑定器分组类型")]
            public Builder.GroupType BindersGroupType;

            [LabelText("绑定器分组名称")]
            [ShowIf(nameof(ShowBindersGroupName))]
            public string BindersGroupName;

            private bool ShowBindersGroupName => BindersGroupType != Builder.GroupType.None;

            private IEnumerable GetBaseTypeDropdown()
            {
                return BaseTypes;
            }
        }

        [Title("默认值设置")]
        public DefaultSettings Default;
        
        [Title("代码格式设置")]
        [LabelText("缩进空格数")]
        public int IndentSpace = 4;

        [Title("类内容生成模板")]
        [LabelText("自动缩进")]
        public bool AutoIndentTemplate = true;

        [LabelText("脚本生成类型：Controller")]
        [TextArea(5, 10)]
        public string ControllerScriptTypeTemplate;

        [LabelText("脚本生成类型：UIPanel")]
        [TextArea(5, 10)]
        public string UIPanelScriptTypeTemplate;

        public string GetIndent()
        {
            var indent = "";
            for (int i = 0; i < IndentSpace; i++)
            {
                indent += " ";
            }
            return indent;
        }

        private const TypeInclusionFilter TypeFilter =
            TypeInclusionFilter.IncludeConcreteTypes | TypeInclusionFilter.IncludeAbstracts;

        [SerializeField, HideInInspector] private SerializationData _serializationData;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref _serializationData);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UnitySerializationUtility.SerializeUnityObject(this, ref _serializationData);
        }
    }
}
