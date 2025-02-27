using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [EditorSettingsAssetPath]
    public class ViewBinderSettings : ScriptableObjectSingleton<ViewBinderSettings>
    {
        [Serializable, HideLabel, InlineProperty, HideReferenceObjectPicker]
        public class PriorityList : ISerializationCallbackReceiver
        {
            [ShowInInspector, NonSerialized]
            [LabelText("优先级")]
            [TypeDrawerSettings(BaseType = typeof(Component), Filter = TypeInclusionFilter.IncludeConcreteTypes)]
            public List<Type> Collection = new List<Type>();

            [HideInInspector, SerializeField]
            private List<string> _serializedCollection = new List<string>();

            public int IndexOf(Type type, bool checkDerive, bool checkBase)
            {
                int i = 0;
                foreach (var t in Collection)
                {
                    if (t == type)
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
                        if (type.IsAssignableFrom(t))
                        {
                            return i;
                        }
                    }

                    if (checkBase)
                    {
                        if (type.IsSubclassOf(t))
                        {
                            return i;
                        }
                    }

                    i++;
                }

                return -1;
            }

            public void OnBeforeSerialize()
            {
                _serializedCollection.Clear();

                foreach (var type in Collection)
                {
                    _serializedCollection.Add(TwoWaySerializationBinder.Default.BindToName(type));
                }
            }

            public void OnAfterDeserialize()
            {
                if (Collection.IsNotNullOrEmpty())
                    return;

                foreach (var type in _serializedCollection)
                {
                    Collection.Add(TwoWaySerializationBinder.Default.BindToType(type));
                }
            }
        }
        
        [Serializable, HideLabel, InlineProperty, HideReferenceObjectPicker]
        public class DefaultSettings
        {
            public ViewBindAccess BindAccess;
            public bool AutoBindName = true;
            public string BindName;

            public bool ProcessBindName = true;

            public bool UseDocumentComment = true;

            [ShowIf(nameof(ShowAutoAddParaToComment))]
            public bool AutoAddParaToComment = true;

            public string Comment;

            private bool ShowAutoAddParaToComment => UseDocumentComment;
        }

        [TitleEx("默认值设置")]
        public DefaultSettings Default;
        
        [TitleEx("优先级设置")]
        public PriorityList Priority;
    }
}
