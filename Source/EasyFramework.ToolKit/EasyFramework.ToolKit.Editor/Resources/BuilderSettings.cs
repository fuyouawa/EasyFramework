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
                return s_baseTypes ??= AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(asm => asm.GetTypes())
                    .Where(t => t.IsSubclassOf(typeof(Component)) && !t.ContainsGenericParameters && t.IsPublic)
                    .ToArray();
                ;
            }
        }

        private static Type[] s_architectureTypes;

        public static Type[] ArchitectureTypes
        {
            get
            {
                return s_architectureTypes ??= AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(asm => asm.GetTypes())
                    .Where(type =>
                        type.IsImplementsOpenGenericType(typeof(Architecture<>)) && !type.IsAbstract &&
                        !type.IsGenericType && type.IsPublic)
                    .ToArray();
            }
        }

        [Title("预设设置")]
        [LabelText("代码生成路径预设")]
        [FolderPath(ParentFolder = "Assets")]
        public string[] GenerateDirectoryPresets = { "Scripts" };
        [LabelText("命名空间预设")]
        public string[] NamespacePresets = { };

        [Title("默认值设置")]
        [LabelText("生成路径")]
        [ValueDropdown(nameof(GenerateDirectoryPresets))]
        public string DefaultGenerateDirectory = "Scripts";

        [LabelText("命名空间")]
        [ValueDropdown(nameof(NamespacePresets))]
        public string DefaultNamespace;

        [LabelText("脚本类型")]
        public Builder.ScriptType DefaultScriptType;

        [LabelText("Controller基类类型")]
        [ShowInInspector]
        [ValueDropdown(nameof(BaseTypes))]
        public Type DefaultControllerBaseType = typeof(MonoBehaviour);

        [LabelText("UIPanel基类类型")]
        [ShowInInspector]
        [ValueDropdown(nameof(BaseTypes))]
        public Type DefaultUIPanelBaseType = Type.GetType("EasyFramework.UIKit.UIPanel, EasyFramework.UIKit");

        [LabelText("架构")]
        [ShowInInspector]
        [ValueDropdown(nameof(ArchitectureTypes))]
        public Type DefaultArchitectureType;

        [LabelText("绑定器分组类型")]
        public Builder.GroupType DefaultBindersGroupType = Builder.GroupType.Title;

        [LabelText("绑定器分组名称")]
        [ShowIf(nameof(ShowDefaultBindersGroupName))]
        public string DefaultBindersGroupName = "Binding";

        private bool ShowDefaultBindersGroupName => DefaultBindersGroupType != Builder.GroupType.None;

        [Title("代码格式设置")]
        [LabelText("缩进空格数")]
        public int IndentSpace = 4;

        [Title("类内容生成模板")]
        [LabelText("自动缩进")]
        public bool AutoIndentTemplate = true;

        [LabelText("Controller脚本模板")]
        [TextArea(5, 10)]
        public string ControllerScriptTemplate = @"
void Awake()
{
}

void Start()
{
}

void Update()
{
}
".Trim();

        [LabelText("UIPanel脚本模板")]
        [TextArea(5, 10)]
        public string UIPanelScriptTemplate = @"
protected override void OnOpen(IPanelData panelData)
{
}

protected override void OnClose()
{
}
".Trim();

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
