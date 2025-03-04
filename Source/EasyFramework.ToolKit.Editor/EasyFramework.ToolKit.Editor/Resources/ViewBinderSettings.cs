using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [EditorSettingsAssetPath]
    [ShowOdinSerializedPropertiesInInspector]
    public class ViewBinderSettings : ScriptableObjectSingleton<ViewBinderSettings>, ISerializationCallbackReceiver
    {
        [HideLabel, InlineProperty, HideReferenceObjectPicker]
        public struct FilteredType
        {
            [HideLabel, ShowInInspector]
            [TypeDrawerSettings(BaseType = typeof(Component), Filter = TypeInclusionFilter.IncludeConcreteTypes)]
            public Type Value;
        }

        [HideLabel, InlineProperty, HideReferenceObjectPicker]
        public struct PriorityList
        {
            [LabelText("优先级列表")]
            public List<FilteredType> Collection;

            public int MatchIndexOf(Type type, bool checkDerive, bool checkBase)
            {
                int i = 0;
                foreach (var t in Collection)
                {
                    if (t.Value == type)
                        return i;

                    i++;
                }

                i = 0;
                foreach (var t in Collection)
                {
                    if (checkDerive)
                    {
                        if (type.IsAssignableFrom(t.Value))
                            return i;
                    }

                    if (checkBase)
                    {
                        if (type.IsSubclassOf(t.Value))
                            return i;
                    }

                    i++;
                }

                return -1;
            }
        }

        [HideLabel, InlineProperty, HideReferenceObjectPicker]
        public struct DefaultSettings
        {
            [LabelText("绑定游戏对象")]
            public bool BindGameObject;
            [LabelText("绑定权限")]
            public ViewBindAccess BindAccess;
            [LabelText("自动绑定名称")]
            public bool AutoBindName;
            [LabelText("处理绑定命名")]
            public bool ProcessBindName;
            [LabelText("使用文档注释")]
            public bool UseDocumentComment;

            [LabelText("自动添加注释段落")]
            [ShowIf(nameof(ShowAutoAddParaToComment))]
            public bool AutoAddParaToComment;

            [LabelText("注释")]
            public string Comment;

            private bool ShowAutoAddParaToComment => UseDocumentComment;
        }

        [TitleEx("默认值设置")]
        public DefaultSettings Default;

        [TitleEx("优先级设置")]
        public PriorityList Priorities;
        
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
