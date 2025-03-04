using System;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [EditorSettingsAssetPath]
    [ShowOdinSerializedPropertiesInInspector]
    public class ViewControllerSettings : ScriptableObjectSingleton<ViewControllerSettings>, ISerializationCallbackReceiver
    {
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
        
        [SerializeField, HideInInspector]
        private SerializationData _serializationData;
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
