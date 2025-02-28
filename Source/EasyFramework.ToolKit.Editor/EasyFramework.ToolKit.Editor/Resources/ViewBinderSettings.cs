using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [EditorSettingsAssetPath]
    public class ViewBinderSettings : ScriptableObjectSingleton<ViewBinderSettings>
    {
        [Serializable, HideLabel, InlineProperty, HideReferenceObjectPicker]
        public struct FilteredType
        {
            [HideLabel, ShowInInspector, NonSerialized]
            [TypeDrawerSettings(BaseType = typeof(Component), Filter = TypeInclusionFilter.IncludeConcreteTypes)]
            public Type Value;
        }

        [Serializable, HideLabel, InlineProperty, HideReferenceObjectPicker]
        public class PriorityList : ISerializationCallbackReceiver
        {
            [ShowInInspector, NonSerialized] [LabelText("优先级列表")]
            public List<FilteredType> Collection = new List<FilteredType>();

            public int MatchIndexOf(Type type, bool checkDerive, bool checkBase)
            {
                int i = 0;
                foreach (var t in Collection)
                {
                    if (t.Value == type)
                    {
                        return i;
                    }

                    i++;
                }

                i = 0;
                foreach (var t in Collection)
                {
                    if (checkDerive)
                    {
                        if (type.IsAssignableFrom(t.Value))
                        {
                            return i;
                        }
                    }

                    if (checkBase)
                    {
                        if (type.IsSubclassOf(t.Value))
                        {
                            return i;
                        }
                    }

                    i++;
                }

                return -1;
            }

            [HideInInspector, SerializeField]
            private List<string> _serializedCollection = new List<string>();

            public void OnBeforeSerialize()
            {
                _serializedCollection.Clear();

                foreach (var type in Collection)
                {
                    var t = type.Value == null
                        ? string.Empty
                        : TwoWaySerializationBinder.Default.BindToName(type.Value);
                    _serializedCollection.Add(t);
                }
            }

            public void OnAfterDeserialize()
            {
                if (Collection.IsNotNullOrEmpty())
                    return;

                foreach (var type in _serializedCollection)
                {
                    var t = type.IsNullOrWhiteSpace()
                        ? null
                        : TwoWaySerializationBinder.Default.BindToType(type);
                    Collection.Add(new FilteredType() { Value = t });
                }
            }
        }

        [Serializable, HideLabel, InlineProperty, HideReferenceObjectPicker]
        public class DefaultSettings
        {
            [LabelText("绑定游戏对象")] public bool BindGameObject = false;
            [LabelText("绑定权限")] public ViewBindAccess BindAccess;
            [LabelText("自动绑定名称")] public bool AutoBindName = true;
            [LabelText("处理绑定命名")] public bool ProcessBindName = true;
            [LabelText("使用文档注释")] public bool UseDocumentComment = true;

            [LabelText("自动添加注释段落")] [ShowIf(nameof(ShowAutoAddParaToComment))]
            public bool AutoAddParaToComment = true;

            [LabelText("注释")] public string Comment;

            private bool ShowAutoAddParaToComment => UseDocumentComment;
        }

        [TitleEx("默认值设置")] public DefaultSettings Default;

        [TitleEx("优先级设置")] public PriorityList Priorities;
    }
}
